using ElPrado.DataFactory;
using ElPrado.DataFactory.Factories;
using ElPrado.DataFactory.Interfaces;
using ElPrado.Workers.Interfaces;
using ElPrado.Workers.Services;
using ElPrado.Workers.Workers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables(prefix: "DOTNET_");
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .UseWindowsService(options => options.ServiceName = "ElPrado.Workers")
    .UseSystemd()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Memory Cache
        services.AddMemoryCache();

        // Connection factory para Firebird
        services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();

        // Repositorios creados con factory
        services.AddSingleton<IConfigServicesRepository, ConfigServicesRepository>();
        services.AddSingleton<IColaEnvioFacturaRepository, ColaEnvioFacturaRepository>();
        services.AddSingleton<IXmlRepository, FirebirdXmlRepository>(); // Data Protection

        // Data Protection
        services.AddDataProtection()
            .SetApplicationName("ElPradoCRM");

        services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(sp =>
            new ConfigureOptions<KeyManagementOptions>(options =>
            {
                options.XmlRepository = sp.GetRequiredService<IXmlRepository>();
            }));

        // DI de servicios necesarios para el worker
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ProcesadorEnviosService>();

        // WorkerStateService como Singleton
        services.AddSingleton<IWorkerStateService, WorkerStateService>();

        // Registrar el EmailWorker
        services.AddHostedService<EmailWorker>();
    })
    .UseSerilog((context, loggerConfig) =>
    {
        loggerConfig
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext();
    })
    .Build();

await host.RunAsync();
