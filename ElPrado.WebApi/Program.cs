using ElPrado.Data;
using ElPrado.Data.Interfaces;
using ElPrado.DataFactory;
using ElPrado.DataFactory.Factories;
using ElPrado.DataFactory.Interfaces;
using ElPrado.Reports.Interfaces;
using ElPrado.Reports.Services;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using ElPrado.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Usar el archivo de configuración según el entorno
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

builder.WebHost.UseConfiguration(configBuilder)
    .UseUrls("http://*:80");

// Configurar Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            new string[] {}
        }
    });
});

// CORS dinámico según el entorno
var policyName = "CorsPolicy";
var allowedOrigins =
    builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ??
    Array.Empty<string>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(policyName, policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(policyName, policy =>
        {
            policy
                .SetIsOriginAllowed(origin =>
                {
                    if (string.IsNullOrEmpty(origin))
                        return false;

                    var host = new Uri(origin).Host;

                    return allowedOrigins.Any(x =>
                        host.Equals(x, StringComparison.OrdinalIgnoreCase) ||
                        host.EndsWith("." + x, StringComparison.OrdinalIgnoreCase));
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
}
/* Versión anterior de CORS, menos segura porque no valida subdominios ni esquemas
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
*/

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });


// Connection factory para Firebird
builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();

// Repositorios creados con factory
builder.Services.AddSingleton<IConfigServicesRepository, ConfigServicesRepository>();
builder.Services.AddSingleton<IXmlRepository, FirebirdXmlRepository>(); //Data Protection

// Data Protection
builder.Services.AddDataProtection()
    .SetApplicationName("ElPradoCRM");

// Configuración correcta (sin BuildServiceProvider)
builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(sp =>
    new ConfigureOptions<KeyManagementOptions>(options =>
    {
        options.XmlRepository = sp.GetRequiredService<IXmlRepository>();
    }));


builder.Services.AddMemoryCache(); // registra IMemoryCache exigido por ReportImageService

// Configuraciones fuertemente tipadas
builder.Services.Configure<ElPrado.Core.Configuration.PdfSettings>(builder.Configuration.GetSection("PdfSettings"));
builder.Services.Configure<ElPrado.Core.Configuration.ImagenSettings>(builder.Configuration.GetSection("ImagenSettings"));

// DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IPdfStorageService, PdfStorageService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();
builder.Services.AddScoped<IReportImageService, ReportImageService>();

// Configuraciones varias
ElPrado.Reports.Configuration.DocSettings.Configurar();

var app = builder.Build();

// Middleware global de errores
app.UseMiddleware<ElPrado.WebApi.MiddleWares.ErrorHandlingMiddleware>();

// Swagger solo en dev/staging
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection(); // Forzar HTTPS en producción
}

app.UseCors(policyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
