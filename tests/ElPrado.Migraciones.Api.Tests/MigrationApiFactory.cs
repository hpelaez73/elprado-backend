using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ElPrado.Migraciones.Api.Tests;

public sealed class MigrationApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(configuration =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Seq:ServerUrl"] = string.Empty
                , ["MigrationAuthentication:AllowInsecureLocalRequests"] = "true"
                , ["MigrationAuthentication:Jwt:Issuer"] = "migration-tests"
                , ["MigrationAuthentication:Jwt:Audience"] = "migration-tests"
                , ["MigrationAuthentication:Jwt:SigningKey"] = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA="
                , ["MigrationAuthentication:Admin:Username"] = "admin"
                , ["MigrationAuthentication:Admin:PasswordHash"] = "pbkdf2-sha512-v1$210000$zhIhjpOMU2YPLMduVgOBWQ==$LRft2ZDzcWv5F6A0MnFp3QA81Pv6BcC/7hz2QEQO/+E="
                , ["MigrationAuthentication:Agents:0:AgentId"] = "agent-one"
                , ["MigrationAuthentication:Agents:0:SecretHash"] = "pbkdf2-sha512-v1$210000$zhIhjpOMU2YPLMduVgOBWQ==$LRft2ZDzcWv5F6A0MnFp3QA81Pv6BcC/7hz2QEQO/+E="
                , ["MigrationAuthentication:Agents:0:Enabled"] = "true"
                , ["MigrationAuthentication:Agents:1:AgentId"] = "agent-two"
                , ["MigrationAuthentication:Agents:1:SecretHash"] = "pbkdf2-sha512-v1$210000$zhIhjpOMU2YPLMduVgOBWQ==$LRft2ZDzcWv5F6A0MnFp3QA81Pv6BcC/7hz2QEQO/+E="
                , ["MigrationAuthentication:Agents:1:Enabled"] = "true"
                , ["MigrationAuthentication:RateLimit:PermitLimit"] = "100"
            }));
    }
}
