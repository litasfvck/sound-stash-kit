using AuthService.DTOs;

namespace AuthService.Helpers
{
    public static class CookieHelper
    {
        private const string AccessTokenCookie = "access_token";
        private const string RefreshTokenCookie = "refresh_token";

        public static void SetTokenCookies(HttpContext httpContext, TokenResponse tokenResponse)
        {
            httpContext.Response.Cookies.Append(
                AccessTokenCookie,
                tokenResponse.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    Path = "/"
                }
            );

            httpContext.Response.Cookies.Append(
                RefreshTokenCookie,
                tokenResponse.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.RefreshExpiresIn),
                    Path = "/auth/refresh"
                }
            );
        }
    }
}