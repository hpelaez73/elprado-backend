using ElPrado.Services;
using System.Security.Claims;

namespace ElPrado.McpApi.Auth;

public sealed class McpUserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public McpUserContextService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public int GetCodCliente() => GetIntegerClaim("CodCliente");

    public int GetCodPropuesta() => GetIntegerClaim("CodPropuesta");

    public int GetCodUsuario() => GetIntegerClaim(ClaimTypes.NameIdentifier);

    public bool EsTesting() => bool.TryParse(GetClaim("EsTesting"), out bool esTesting) && esTesting;

    private int GetIntegerClaim(string claimType) => int.TryParse(GetClaim(claimType), out int value) ? value : 0;

    private string? GetClaim(string claimType) => _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
}
