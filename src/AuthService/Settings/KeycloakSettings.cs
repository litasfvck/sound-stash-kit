namespace AuthService.Settings
{
    public class KeycloakSettings
    {
        public const string KeycloakSettingsName = "Keycloak";
        public string BaseUrl { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        public string TokenUrl => 
            $"{BaseUrl}/realms/{Realm}/protocol/openid-connect/token";
        public string AdminUsersUrl => 
            $"{BaseUrl}/admin/realms/{Realm}/users";
        public string AdminTokenUrl => 
            $"{BaseUrl}/realms/master/protocol/openid-connect/token";
    }
}