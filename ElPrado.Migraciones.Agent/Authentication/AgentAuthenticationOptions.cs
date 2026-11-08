using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace ElPrado.Migraciones.Agent.Authentication;

public sealed class AgentAuthenticationOptions
{
    public const string SectionName = "MigrationAgentAuthentication";
    [Required] public string ApiUrl { get; init; } = string.Empty;
    [Required] public string AgentId { get; init; } = string.Empty;
    [Required] public string AgentSecret { get; init; } = string.Empty;
    public int RenewalMarginSeconds { get; init; } = 120;
    public RetryOptions Retry { get; init; } = new();

    public void Validate()
    {
        var errors = new List<string>();
        if (!Uri.TryCreate(ApiUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps) errors.Add("ApiUrl debe ser HTTPS absoluto.");
        if (string.IsNullOrWhiteSpace(AgentId) || string.IsNullOrWhiteSpace(AgentSecret)) errors.Add("AgentId y AgentSecret son obligatorios.");
        if (RenewalMarginSeconds is < 30 or > 300) errors.Add("RenewalMarginSeconds debe estar entre 30 y 300 segundos.");
        Retry.Validate(errors);
        if (errors.Count > 0) throw new OptionsValidationException(SectionName, typeof(AgentAuthenticationOptions), errors);
    }
}

public sealed class RetryOptions
{
    public int BaseDelaySeconds { get; init; } = 1;
    public int MaxDelaySeconds { get; init; } = 60;
    public int JitterPercent { get; init; } = 20;
    public void Validate(List<string> errors)
    {
        if (BaseDelaySeconds is < 1 or > 10 || MaxDelaySeconds is < 30 or > 300 || MaxDelaySeconds < BaseDelaySeconds || JitterPercent is < 0 or > 25)
            errors.Add("La configuración de reintentos es inválida.");
    }
}
