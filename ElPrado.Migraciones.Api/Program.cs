using ElPrado.Migraciones.Api.Features.Health;
using ElPrado.Migraciones.Api.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteToOptionalSeq(context.Configuration));

    builder.Services.AddMigrationApiServices();

    var app = builder.Build();

    app.Lifetime.ApplicationStarted.Register(() =>
        Log.Information("Migration API started in {Environment}", app.Environment.EnvironmentName));
    app.Lifetime.ApplicationStopping.Register(() =>
        Log.Information("Migration API is stopping"));

    app.UseExceptionHandler(exceptionApplication =>
    {
        exceptionApplication.Run(async context =>
        {
            var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

            if (exception is not null)
            {
                Log.Error(exception, "Unhandled exception while processing {RequestPath}", context.Request.Path);
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await Results.Problem(statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(context);
        });
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.MapHealthEndpoints();

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program
{
}
