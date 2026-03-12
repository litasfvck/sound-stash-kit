using SoundStashKit.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SoundStashKit.Services.PackService;
using SoundStashKit.Services.FakeUserService;
using SoundStashKit.Services.SampleService;
using SoundStashKit.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SoundstashDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<IFakeUser, FakeUser>();
builder.Services.AddScoped<ISampleService,SampleService>();
builder.Services.AddScoped<IPackService, PackService>();

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
app.MapSampleEndpoints();
// app.MapPackEndpoints();

app.Run();