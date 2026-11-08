using System.Security.Claims;
using ElPrado.Migraciones.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElPrado.Migraciones.Api.Tests;

public sealed class AgentAuthorizationHandlerTests
{
    [Fact]
    public async Task Revoking_one_agent_denies_only_its_existing_token()
    {
        var monitor = new MutableOptionsMonitor(new MigrationAuthenticationOptions
        {
            Agents = new List<AgentRegistration>
            {
                new() { AgentId = "one", SecretHash = ValidHash, Enabled = true },
                new() { AgentId = "two", SecretHash = ValidHash, Enabled = true }
            }
        });
        var handler = new AgentAuthorizationHandler(monitor);
        Assert.True((await Authorize(handler, "one")).HasSucceeded);
        monitor.Value = new MigrationAuthenticationOptions { Agents = new List<AgentRegistration> { new() { AgentId = "one", SecretHash = ValidHash, Enabled = false }, new() { AgentId = "two", SecretHash = ValidHash, Enabled = true } } };
        Assert.False((await Authorize(handler, "one")).HasSucceeded);
        Assert.True((await Authorize(handler, "two")).HasSucceeded);
    }

    private static async Task<AuthorizationHandlerContext> Authorize(AgentAuthorizationHandler handler, string agentId)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, agentId), new Claim(JwtTokenIssuer.IdentityClaim, "agent") }, "test");
        var context = new AuthorizationHandlerContext(new[] { new EnabledAgentRequirement() }, new ClaimsPrincipal(identity), null);
        await handler.HandleAsync(context);
        return context;
    }

    private const string ValidHash = "pbkdf2-sha512-v1$210000$zhIhjpOMU2YPLMduVgOBWQ==$LRft2ZDzcWv5F6A0MnFp3QA81Pv6BcC/7hz2QEQO/+E=";

    private sealed class MutableOptionsMonitor : IOptionsMonitor<MigrationAuthenticationOptions>
    {
        public MutableOptionsMonitor(MigrationAuthenticationOptions value) => Value = value;
        public MigrationAuthenticationOptions Value { get; set; }
        public MigrationAuthenticationOptions CurrentValue => Value;
        public MigrationAuthenticationOptions Get(string? name) => Value;
        public IDisposable OnChange(Action<MigrationAuthenticationOptions, string?> listener) => Empty.Instance;
        private sealed class Empty : IDisposable { public static readonly Empty Instance = new(); public void Dispose() { } }
    }
}
