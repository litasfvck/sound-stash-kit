using AuthService.DTOs;

namespace AuthService.Services.KeycloakService;

public interface IKeycloakService
{
    Task<TokenResponse> LoginAsync(string email, string password);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> RegisterUserAsync(RegisterRequest request);
    Task RevokeTokenAsync(string refreshToken);
}