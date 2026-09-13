using ElPrado.Data;
using ElPrado.Data.Interfaces;
using ElPrado.DataFactory;
using ElPrado.DataFactory.Interfaces;
using ElPrado.McpApi.Auth;
using ElPrado.McpApi.Contracts;
using ElPrado.McpApi.Endpoints;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

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

JwtOptions jwtOptions = JwtOptions.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<McpJwtTokenIssuer>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Issuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// Connections are created only when a future business adapter resolves and uses them.
builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, McpUserContextService>();
builder.Services.AddScoped<PropuestasService>();
builder.Services.AddScoped<CuentasCorrientesService>();
builder.Services.AddScoped<ServiciosModelosService>();
builder.Services.AddScoped<ComprobantesService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(ApiResponse<HealthStatus>.Success(
    new HealthStatus("ElPrado.McpApi", "available"))));

app.MapLogin();
app.MapBusinessOperations();

app.Run();

public partial class Program
{
}
