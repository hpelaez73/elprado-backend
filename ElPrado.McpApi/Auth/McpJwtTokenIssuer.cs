using ElPrado.Dto.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElPrado.McpApi.Auth;

public sealed class McpJwtTokenIssuer
{
    private readonly JwtOptions _options;
    private readonly IConfiguration _configuration;

    public McpJwtTokenIssuer(JwtOptions options, IConfiguration configuration)
    {
        _options = options;
        _configuration = configuration;
    }

    public IssuedAccessToken Issue(DtoLogin login)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        DateTimeOffset expiresAt = now.Add(_options.AccessTokenLifetime);
        bool esTesting = _configuration.GetConnectionString("DefaultConnection")?.Contains("elprado_dev", StringComparison.OrdinalIgnoreCase) ?? false;
        Claim[] claims =
        {
            new(ClaimTypes.NameIdentifier, login.CodUsuario.ToString()),
            new(ClaimTypes.Name, login.Nombre),
            new("CodUsuario", login.CodUsuario.ToString()),
            new("CodCliente", "0"),
            new("CodPropuesta", "0"),
            new("EsTesting", esTesting.ToString())
        };
        SigningCredentials credentials = new(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            issuer: _options.Issuer,
            audience: _options.Issuer,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new IssuedAccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

public sealed record IssuedAccessToken(string Token, DateTimeOffset ExpiresAt);
