using System.Security.Cryptography;
using ElPrado.Migraciones.Api.Authentication;
using ElPrado.Migraciones.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ElPrado.Migraciones.Api.Features.Authentication;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth");
        group.MapPost("/admin/login", AdminLogin).RequireRateLimiting("token");
        group.MapPost("/agent/token", AgentToken).RequireRateLimiting("token");
        group.MapGet("/me", AdminMe).RequireAuthorization("AdminOnly");
        group.MapGet("/agent/me", AgentMe).RequireAuthorization("AgentOnly");
        return endpoints;
    }

    private static IResult AdminLogin(AdminLoginRequest request, IOptions<MigrationAuthenticationOptions> options, JwtTokenIssuer issuer)
    {
        var admin = options.Value.Admin;
        var validUser = CryptographicOperations.FixedTimeEquals(System.Text.Encoding.UTF8.GetBytes(request.Username ?? string.Empty), System.Text.Encoding.UTF8.GetBytes(admin.Username));
        if (!validUser || !Pbkdf2PasswordHasher.Verify(request.Password ?? string.Empty, admin.PasswordHash)) return Results.Unauthorized();
        var token = issuer.IssueAdminToken(admin.Username);
        return Results.Ok(new TokenResponse(token.AccessToken, token.ExpiresAt));
    }

    private static IResult AgentToken(AgentTokenRequest request, IOptionsMonitor<MigrationAuthenticationOptions> options, JwtTokenIssuer issuer)
    {
        var agent = options.CurrentValue.Agents.FirstOrDefault(item => string.Equals(item.AgentId, request.AgentId, StringComparison.Ordinal));
        if (agent is null || !agent.Enabled || !Pbkdf2PasswordHasher.Verify(request.AgentSecret ?? string.Empty, agent.SecretHash)) return Results.Unauthorized();
        var token = issuer.IssueAgentToken(agent.AgentId);
        return Results.Ok(new AgentTokenResponse(token.AccessToken, token.ExpiresAt));
    }

    private static IResult AdminMe(HttpContext context) => Results.Ok(new IdentityResponse(Subject(context) ?? "admin", "admin"));
    private static IResult AgentMe(HttpContext context) => Results.Ok(new IdentityResponse(Subject(context) ?? string.Empty, "agent"));
    private static string? Subject(HttpContext context) => context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;

    private sealed record AdminLoginRequest(string? Username, string? Password);
    private sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
    private sealed record IdentityResponse(string Subject, string Identity);
}
