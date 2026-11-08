using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ElPrado.Migraciones.Api.Authentication;

public sealed class JwtTokenIssuer
{
    public const string IdentityClaim = "migration_identity";
    private readonly IOptions<MigrationAuthenticationOptions> _options;
    public JwtTokenIssuer(IOptions<MigrationAuthenticationOptions> options) => _options = options;

    public IssuedToken IssueAdminToken(string username) => Issue(username, "admin", _options.Value.Jwt.AdminLifetimeMinutes);
    public IssuedToken IssueAgentToken(string agentId) => Issue(agentId, "agent", _options.Value.Jwt.AgentLifetimeMinutes);

    private IssuedToken Issue(string subject, string identity, int lifetimeMinutes)
    {
        var jwt = _options.Value.Jwt;
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(lifetimeMinutes);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(jwt.SigningKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience,
            new[] { new Claim(JwtRegisteredClaimNames.Sub, subject), new Claim(IdentityClaim, identity) },
            now.UtcDateTime, expiresAt.UtcDateTime, credentials);
        return new IssuedToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAt);
