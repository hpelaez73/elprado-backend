using ElPrado.Migraciones.Agent.Workers;
using Serilog;

namespace ElPrado.Migraciones.Agent.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrationAgentServices(this IServiceCollection services)
    {
        services.AddHostedService<AgentWorker>();

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
