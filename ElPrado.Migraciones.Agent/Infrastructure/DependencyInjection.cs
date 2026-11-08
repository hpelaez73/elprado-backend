using ElPrado.Migraciones.Agent.Workers;
using ElPrado.Migraciones.Agent.Authentication;
using Serilog;

namespace ElPrado.Migraciones.Agent.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrationAgentServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (configuration is not null)
        {
            services.AddOptions<AgentAuthenticationOptions>()
                .Bind(configuration.GetSection(AgentAuthenticationOptions.SectionName))
                .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "La configuración de autenticación del agente es inválida.")
                .ValidateOnStart();
            var options = configuration.GetSection(AgentAuthenticationOptions.SectionName).Get<AgentAuthenticationOptions>() ?? new AgentAuthenticationOptions();
            options.Validate();
            services.AddHttpClient<IAgentTokenProvider, AgentTokenProvider>(client => client.BaseAddress = new Uri(options.ApiUrl));
        }
        services.AddHostedService<AgentWorker>();
        services.AddWindowsService();
        services.AddSystemd();

        return services;
    }

    public static LoggerConfiguration WriteToOptionalSeq(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        var serverUrl = configuration["Seq:ServerUrl"];

        return string.IsNullOrWhiteSpace(serverUrl)
            ? loggerConfiguration
            : loggerConfiguration.WriteTo.Seq(serverUrl);
    }
}
