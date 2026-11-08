using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

namespace ElPrado.Migraciones.Api.Authentication;

public sealed class MigrationAuthenticationOptions
{
    public const string SectionName = "MigrationAuthentication";
    public JwtOptions Jwt { get; init; } = new();
    public AdminOptions Admin { get; init; } = new();
    public List<AgentRegistration> Agents { get; init; } = new();
    public Pbkdf2Options Pbkdf2 { get; init; } = new();
    public TokenRateLimitOptions RateLimit { get; init; } = new();
    public TrustedProxyOptions TrustedProxies { get; init; } = new();
    public bool AllowInsecureLocalRequests { get; init; }

    public void Validate()
    {
        Jwt.Validate();
        Admin.Validate();
        Pbkdf2.Validate();
        RateLimit.Validate();
        if (Agents.GroupBy(agent => agent.AgentId, StringComparer.Ordinal).Any(group => group.Count() != 1))
            throw new OptionsValidationException(SectionName, typeof(MigrationAuthenticationOptions), new[] { "Los AgentId deben ser únicos." });
        foreach (var agent in Agents) agent.Validate();
        foreach (var address in TrustedProxies.KnownProxies) ParseAddress(address, "KnownProxies");
        foreach (var network in TrustedProxies.KnownNetworks) ParseNetwork(network);
    }

    private static void ParseAddress(string value, string name)
    {
        if (!IPAddress.TryParse(value, out _))
            throw new OptionsValidationException(SectionName, typeof(MigrationAuthenticationOptions), new[] { $"{name} contiene una IP inválida." });
    }

    private static void ParseNetwork(string value)
    {
        var parts = value.Split('/');
        if (parts.Length != 2 || !IPAddress.TryParse(parts[0], out var address) || !int.TryParse(parts[1], out var prefix) ||
            prefix < 0 || prefix > (address.AddressFamily == AddressFamily.InterNetwork ? 32 : 128))
            throw new OptionsValidationException(SectionName, typeof(MigrationAuthenticationOptions), new[] { "KnownNetworks contiene una red CIDR inválida." });
    }
}

public sealed class JwtOptions
{
    [Required] public string Issuer { get; init; } = string.Empty;
    [Required] public string Audience { get; init; } = string.Empty;
    [Required] public string SigningKey { get; init; } = string.Empty;
    public int AdminLifetimeMinutes { get; init; } = 30;
    public int AgentLifetimeMinutes { get; init; } = 15;

    public void Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Issuer)) errors.Add("Jwt:Issuer es obligatorio.");
        if (string.IsNullOrWhiteSpace(Audience)) errors.Add("Jwt:Audience es obligatorio.");
        try { if (Convert.FromBase64String(SigningKey).Length < 32) errors.Add("Jwt:SigningKey debe contener al menos 256 bits Base64."); }
        catch (FormatException) { errors.Add("Jwt:SigningKey debe ser Base64 válido."); }
        if (AdminLifetimeMinutes is < 5 or > 60) errors.Add("La vida admin debe estar entre 5 y 60 minutos.");
        if (AgentLifetimeMinutes is < 5 or > 30) errors.Add("La vida agent debe estar entre 5 y 30 minutos.");
        if (errors.Count > 0) throw new OptionsValidationException(MigrationAuthenticationOptions.SectionName, typeof(MigrationAuthenticationOptions), errors);
    }
}

public sealed class AdminOptions
{
    [Required] public string Username { get; init; } = string.Empty;
    [Required] public string PasswordHash { get; init; } = string.Empty;
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Username) || !Pbkdf2PasswordHasher.IsValidHash(PasswordHash))
            throw new OptionsValidationException(MigrationAuthenticationOptions.SectionName, typeof(MigrationAuthenticationOptions), new[] { "Admin:Username y Admin:PasswordHash son obligatorios." });
    }
}

public sealed class AgentRegistration
{
    [Required] public string AgentId { get; init; } = string.Empty;
    [Required] public string SecretHash { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AgentId) || !Pbkdf2PasswordHasher.IsValidHash(SecretHash))
            throw new OptionsValidationException(MigrationAuthenticationOptions.SectionName, typeof(MigrationAuthenticationOptions), new[] { "Cada agente requiere AgentId y SecretHash." });
    }
}

public sealed class Pbkdf2Options
{
    public int Iterations { get; init; } = 210000;
    public void Validate()
    {
        if (Iterations is < 210000 or > 1000000)
            throw new OptionsValidationException(MigrationAuthenticationOptions.SectionName, typeof(MigrationAuthenticationOptions), new[] { "PBKDF2:Iterations debe estar entre 210000 y 1000000." });
    }
}

public sealed class TokenRateLimitOptions
{
    public int PermitLimit { get; init; } = 5;
    public int WindowSeconds { get; init; } = 60;
    public void Validate()
    {
        if (PermitLimit is < 1 or > 100 || WindowSeconds is < 1 or > 3600)
            throw new OptionsValidationException(MigrationAuthenticationOptions.SectionName, typeof(MigrationAuthenticationOptions), new[] { "Los límites de rate limiting son inválidos." });
    }
}

public sealed class TrustedProxyOptions
{
    public List<string> KnownProxies { get; init; } = new();
    public List<string> KnownNetworks { get; init; } = new();
}
