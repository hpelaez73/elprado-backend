namespace ElPrado.McpApi.Contracts;

public sealed record LoginUsuarioRequest(string Alias, string Clave);

public sealed record LoginUsuarioResponse(int CodUsuario, string Nombre, string Token, DateTimeOffset ExpiresAt);
