using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuthService.DTOs;
using AuthService.Settings;
using Microsoft.Extensions.Options;

namespace AuthService.Services.KeycloakService;

public class KeycloakService(
        HttpClient httpClient,
        IOptions<KeycloakSettings> settings,
        ILogger<KeycloakService> logger) : IKeycloakService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly KeycloakSettings _settings = settings.Value;

    // --- LOGIN ---
    // Используем grant_type=password (Resource Owner Password Credentials)
    // Это позволяет нашему бэкенду логиниться "от имени" пользователя
    public async Task<TokenResponse> LoginAsync(string email, string password)
    {
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["username"] = email,  // Keycloak принимает email как username если так настроено
            ["password"] = password,
            ["scope"] = "openid"
        };

        return await RequestTokenAsync(_settings.TokenUrl, formData);
    }

    // --- REFRESH ---
    // Клиент присылает refresh token (из httpOnly cookie), мы идём в Keycloak за новой парой токенов
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["refresh_token"] = refreshToken
        };

        return await RequestTokenAsync(_settings.TokenUrl, formData);
    }

    // --- REVOKE (LOGOUT) ---
    // Инвалидируем refresh token в Keycloak, чтобы его нельзя было использовать
    public async Task RevokeTokenAsync(string refreshToken)
    {
        var revokeUrl = $"{_settings.BaseUrl}/realms/{_settings.Realm}/protocol/openid-connect/logout";

        var formData = new Dictionary<string, string>
        {
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["refresh_token"] = refreshToken
        };

        var response = await _httpClient.PostAsync(
            revokeUrl,
            new FormUrlEncodedContent(formData));

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to revoke token. Status: {Status}", response.StatusCode);
        }
    }

    // --- REGISTER ---
    // Создание пользователя через Admin API требует отдельного шага:
    // сначала получаем admin-токен, потом создаём пользователя
    public async Task<bool> RegisterUserAsync(RegisterRequest request)
    {
        var adminToken = await GetAdminTokenAsync();
        if (adminToken is null) return false;

        // Структура тела запроса для создания пользователя в Keycloak
        var userPayload = new
        {
            username = request.Username,
            email = request.Email,
            firstName = request.FirstName,
            lastName = request.LastName,
            enabled = true,
            emailVerified = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = request.Password,
                    temporary = false  // false = пользователь не обязан менять пароль
                }
            }
        };

        var json = JsonSerializer.Serialize(userPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Добавляем admin Bearer токен в заголовок
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.AdminUsersUrl);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        httpRequest.Content = content;

        var response = await _httpClient.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogWarning(
                "Failed to create user in Keycloak. Status: {Status}, Error: {Error}",
                response.StatusCode,
                error);
            return false;
        }

        return true; // 201 Created = успех
    }

    // --- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ---

    private async Task<TokenResponse> RequestTokenAsync(
        string url,
        Dictionary<string, string> formData)
    {
        var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(formData));

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                "Keycloak token request failed. Status: {Status}, Body: {Body}",
                response.StatusCode,
                responseContent);
            throw new HttpRequestException(
                $"Keycloak returned {response.StatusCode}: {responseContent}");
        }

        var keycloakResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent)
            ?? throw new InvalidOperationException("Failed to deserialize Keycloak token response");

        return new TokenResponse(
            keycloakResponse.AccessToken,
            keycloakResponse.RefreshToken,
            keycloakResponse.ExpiresIn,
            keycloakResponse.RefreshExpiresIn,
            keycloakResponse.TokenType);
    }

    // Получаем токен для Admin API через client_credentials
    // Это "машинный" OAuth2 flow — без участия пользователя
    private async Task<string?> GetAdminTokenAsync()
    {
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _settings.AdminUsername,
            ["password"] = _settings.AdminPassword
        };

        var adminTokenUrl = $"{_settings.BaseUrl}/realms/master/protocol/openid-connect/token";

        try
        {
            var response = await _httpClient.PostAsync(
            adminTokenUrl, 
            new FormUrlEncodedContent(formData));
            
            var content = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<KeycloakTokenResponse>(content);
            return token?.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get admin token from Keycloak");
            return null;
        }
    }
}