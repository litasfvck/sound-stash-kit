using AuthService.Endpoints;
using AuthService.Services.KeycloakService;
using AuthService.Settings;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<KeycloakSettings>(
    builder.Configuration.GetSection(KeycloakSettings.KeycloakSettingsName));

builder.Services.AddHttpClient<KeycloakService>();
builder.Services.AddScoped<IKeycloakService, KeycloakService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.BluePlanet;
    });
}

// Эндпоинты
app.MapAuthEndpoints();

app.Run();
