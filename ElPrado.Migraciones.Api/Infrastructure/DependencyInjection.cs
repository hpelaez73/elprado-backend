using Serilog;

namespace ElPrado.Migraciones.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMigrationApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();

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
