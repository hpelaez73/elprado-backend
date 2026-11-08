using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using ElPrado.Migraciones.Api.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace ElPrado.Migraciones.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrationApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MigrationAuthenticationOptions>()
            .Bind(configuration.GetSection(MigrationAuthenticationOptions.SectionName))
            .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "La configuración de autenticación de Migraciones es inválida.")
            .ValidateOnStart();
        services.AddSingleton<JwtTokenIssuer>();
        services.AddSingleton<IAuthorizationHandler, AgentAuthorizationHandler>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme).Configure<IOptions<MigrationAuthenticationOptions>>((jwt, authentication) =>
        {
            var options = authentication.Value;
            jwt.RequireHttpsMetadata = true;
            jwt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(options.Jwt.SigningKey)),
                ValidateIssuer = true, ValidIssuer = options.Jwt.Issuer,
                ValidateAudience = true, ValidAudience = options.Jwt.Audience,
                ValidateLifetime = true, ClockSkew = TimeSpan.Zero,
                NameClaimType = "sub"
            };
        });
        services.AddAuthorization(authorization =>
        {
            authorization.AddPolicy("AdminOnly", policy => policy.RequireAuthenticatedUser().RequireClaim(JwtTokenIssuer.IdentityClaim, "admin"));
            authorization.AddPolicy("AgentOnly", policy => policy.RequireAuthenticatedUser().RequireClaim(JwtTokenIssuer.IdentityClaim, "agent").AddRequirements(new EnabledAgentRequirement()));
        });
        services.AddRateLimiter(rateLimits =>
        {
            rateLimits.AddPolicy("token", httpContext =>
            {
                var settings = httpContext.RequestServices.GetRequiredService<IOptions<MigrationAuthenticationOptions>>().Value.RateLimit;
                var partition = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(partition, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = settings.PermitLimit, Window = TimeSpan.FromSeconds(settings.WindowSeconds), QueueLimit = 0, AutoReplenishment = true
                });
            });
            rateLimits.OnRejected = static (context, _) => { context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests; return ValueTask.CompletedTask; };
        });
        services.AddOptions<ForwardedHeadersOptions>().Configure<IOptions<MigrationAuthenticationOptions>>((headers, authentication) =>
        {
            var options = authentication.Value;
            headers.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            headers.KnownProxies.Clear(); headers.KnownNetworks.Clear();
            foreach (var proxy in options.TrustedProxies.KnownProxies) headers.KnownProxies.Add(IPAddress.Parse(proxy));
            foreach (var network in options.TrustedProxies.KnownNetworks)
            {
                var parts = network.Split('/');
                headers.KnownNetworks.Add(new IPNetwork(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
            }
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();

        return services;
    }

    public static LoggerConfiguration WriteToOptionalSeq(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        var serverUrl = configuration["Seq:ServerUrl"];

        return string.IsNullOrWhiteSpace(serverUrl)
            ? loggerConfiguration
            : loggerConfiguration.WriteTo.Seq(serverUrl);
    }
}
