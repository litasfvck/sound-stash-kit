namespace AuthService.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string KeycloakId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}