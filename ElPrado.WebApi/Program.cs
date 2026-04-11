using ElPrado.Reports.Interfaces;
using ElPrado.Reports.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var policyName = "CorsPolicy";
/*
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName, corsBuilder =>
    {
        if (builder.Environment.IsDevelopment())
        {
            corsBuilder
                .SetIsOriginAllowed(_ => true) // permite cualquier origen en dev
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            corsBuilder
                .WithOrigins(allowedOrigins ?? Array.Empty<string>()) // solo orígenes confiables en prod
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});
*/
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

builder.Services.AddMemoryCache(); // registra IMemoryCache exigido por ReportImageService

builder.Services.Configure<ElPrado.Core.Configuration.PdfSettings>(builder.Configuration.GetSection("PdfSettings"));
builder.Services.Configure<ElPrado.Core.Configuration.ImagenSettings>(builder.Configuration.GetSection("ImagenSettings"));
builder.Services.Configure<ElPrado.Core.Configuration.ImagenSettings>(builder.Configuration.GetSection("BaseUrl"));

// DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ElPrado.Data.IUnitOfWork, ElPrado.Data.UnitOfWork>();
builder.Services.AddScoped<ElPrado.Services.IUserContextService, ElPrado.WebApi.UserContextService>();
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
