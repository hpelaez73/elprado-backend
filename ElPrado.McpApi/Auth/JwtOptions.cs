namespace ElPrado.McpApi.Auth;

public sealed record JwtOptions(string Key, string Issuer, TimeSpan AccessTokenLifetime)
{
    public static JwtOptions FromConfiguration(IConfiguration configuration)
    {
        string key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");
        string issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Falta configurar Jwt:Issuer.");
        int lifetimeHours = configuration.GetValue<int?>("Jwt:AccessTokenLifetimeHours") ?? 24;

        if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Jwt:Key no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(issuer)) throw new InvalidOperationException("Jwt:Issuer no puede estar vacío.");
        if (lifetimeHours <= 0) throw new InvalidOperationException("Jwt:AccessTokenLifetimeHours debe ser mayor que cero.");

        return new JwtOptions(key, issuer, TimeSpan.FromHours(lifetimeHours));
    }
}
