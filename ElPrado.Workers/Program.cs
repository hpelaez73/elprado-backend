using ElPrado.Data;
using ElPrado.Data.Factories;
using ElPrado.Data.Interfaces;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using ElPrado.Services.Workers;
using ElPrado.Workers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Options;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService() // Para ejecutarse como servicio Windows
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Connection factory para Firebird
        services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();

        // Repositorios creados con factory
        services.AddSingleton<IConfigServicesRepository, ConfigServicesRepository>();
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserContextService, WorkerUserContextService>(); // Versión especial para workers
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
