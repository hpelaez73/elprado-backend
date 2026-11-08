namespace ElPrado.Migraciones.Contracts;

public sealed record AgentTokenRequest(string AgentId, string AgentSecret);

public sealed record AgentTokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
