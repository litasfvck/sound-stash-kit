using System.IdentityModel.Tokens.Jwt;
using AuthService.DTOs;
using AuthService.Helpers;
using AuthService.Services.KeycloakService;

namespace AuthService.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/auth");

            group.MapPost("/register", Register);
            group.MapPost("/login", Login);
            group.MapPost("/refresh", Refresh);
            group.MapPost("/logout", Logout);
            group.MapGet("/me", Me);
        }

        private static async Task<IResult> Register(
            RegisterRequest request,
            IKeycloakService keycloakService)
        {
            var success = await keycloakService.RegisterUserAsync(request);
            if (!success) return TypedResults.BadRequest(new { message = "Registration failed" });
            return TypedResults.Ok(success);
        }

        private static async Task<IResult> Login(
            LoginRequest request,
            IKeycloakService keycloakService,
            HttpContext httpContext)
        {
            var tokenResponse = await keycloakService.LoginAsync(request.Email, request.Password);
            CookieHelper.SetTokenCookies(httpContext, tokenResponse);

            return TypedResults.Ok();   
        }

        private static async Task<IResult> Refresh(
            IKeycloakService keycloakService,
            HttpContext httpContext)
        {
            var refreshToken = httpContext.Request.Cookies["refresh_token"];
            if (refreshToken == null)
            {
                return TypedResults.Unauthorized();
            }

            var tokenResponse = await keycloakService.RefreshTokenAsync(refreshToken);
            CookieHelper.SetTokenCookies(httpContext, tokenResponse);

            return TypedResults.Ok();
        }

        private static async Task<IResult> Logout(
            IKeycloakService keycloakService,
            HttpContext httpContext)
        {
            var refreshToken = httpContext.Request.Cookies["refresh_token"];

            if (refreshToken != null)
            {
                await keycloakService.RevokeTokenAsync(refreshToken);
            }

            httpContext.Response.Cookies.Delete("access_token");
            httpContext.Response.Cookies.Delete("refresh_token");

            return TypedResults.Ok();
        }

        private static IResult Me(HttpContext httpContext)
        {
            var token = httpContext.Request.Cookies["access_token"];
            if (token is null) return Results.Unauthorized();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                return Results.Ok(new
                {
                    KeycloakId = jwt.Subject,
                    Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    Username = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
                    FirstName = jwt.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value,
                    LastName = jwt.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value,
                });
            }
            catch (Exception)
            {
                return Results.Unauthorized();
            }
        }        
    }
}