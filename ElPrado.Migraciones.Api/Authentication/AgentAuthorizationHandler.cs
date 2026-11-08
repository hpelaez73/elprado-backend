using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ElPrado.Migraciones.Api.Authentication;

public sealed class EnabledAgentRequirement : IAuthorizationRequirement { }

public sealed class AgentAuthorizationHandler : AuthorizationHandler<EnabledAgentRequirement>
{
    private readonly IOptionsMonitor<MigrationAuthenticationOptions> _options;
    public AgentAuthorizationHandler(IOptionsMonitor<MigrationAuthenticationOptions> options) => _options = options;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EnabledAgentRequirement requirement)
    {
        var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub");
        if (context.User.FindFirstValue(JwtTokenIssuer.IdentityClaim) == "agent" && !string.IsNullOrWhiteSpace(id) &&
            _options.CurrentValue.Agents.Any(agent => agent.Enabled && string.Equals(agent.AgentId, id, StringComparison.Ordinal)))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
