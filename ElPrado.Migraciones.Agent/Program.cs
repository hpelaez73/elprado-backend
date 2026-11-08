using ElPrado.Migraciones.Agent.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddMigrationAgentServices();

    builder.Services.AddSerilog((services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteToOptionalSeq(builder.Configuration));

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Agent terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
