using ElPrado.Data;
using ElPrado.Data.Interfaces;
using ElPrado.DataFactory;
using ElPrado.DataFactory.Interfaces;
using ElPrado.McpApi.Contracts;
using ElPrado.McpApi.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Keep the precedence explicit: base file, environment file, then environment variables.
builder.Configuration.Sources.Clear();
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

// Connections are created only when a future business adapter resolves and uses them.
builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();
var corsEnabled = allowedOrigins.Length > 0;

if (corsEnabled)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ConfiguredOrigins", policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
    });
}

var app = builder.Build();

app.Logger.LogInformation("ElPrado.McpApi started in {Environment} environment", app.Environment.EnvironmentName);

if (corsEnabled)
{
    app.UseCors("ConfiguredOrigins");
}

app.MapGet("/health", () => Results.Ok(ApiResponse<HealthStatus>.Success(
    new HealthStatus("ElPrado.McpApi", "available"))));

app.MapBusinessOperations();

app.Run();

public partial class Program
{
}
