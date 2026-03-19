namespace AuthService.DTOs;

public record UserDto(
    Guid Id, 
    string KeycloakId, 
    string Username, 
    string Email, 
    string FirstName, 
    string LastName
);

public record RegisterRequest(
    string Username, 
    string Email, 
    string Password, 
    string FirstName, 
    string LastName
);

public record LoginRequest(
    string Email, 
    string Password
);

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    int RefreshExpiresIn,
    string TokenType
);