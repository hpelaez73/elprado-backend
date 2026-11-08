using ElPrado.Migraciones.Agent.Infrastructure;
using ElPrado.Migraciones.Agent.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ElPrado.Migraciones.Agent.Tests;

public sealed class AgentWorkerTests
{
    [Fact]
    public void AddMigrationAgentServices_registers_agent_worker()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddMigrationAgentServices();
        using var host = builder.Build();

        Assert.Contains(host.Services.GetServices<IHostedService>(), service => service is AgentWorker);
    }

    [Fact]
    public async Task Host_starts_agent_worker()
    {
        var signals = new LifecycleSignals();
        using var host = CreateHost(signals);

        await host.StartAsync();

        await signals.Started;
        await host.StopAsync();
    }

    [Fact]
    public async Task Agent_worker_stops_when_its_execution_is_cancelled()
    {
        var signals = new LifecycleSignals();
        using var loggerFactory = LoggerFactory.Create(logging => logging.AddProvider(signals));
        var worker = new AgentWorker(loggerFactory.CreateLogger<AgentWorker>());

        await worker.StartAsync(CancellationToken.None);
        await signals.Started;
        await worker.StopAsync(CancellationToken.None);

        await signals.Stopped;
    }

    [Fact]
    public async Task Host_stops_agent_worker_without_errors()
    {
        var signals = new LifecycleSignals();
        using var host = CreateHost(signals);

        await host.StartAsync();
        await signals.Started;
        await host.StopAsync();

        await signals.Stopped;
    }

    private static IHost CreateHost(LifecycleSignals signals)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(signals);
        builder.Services.AddMigrationAgentServices();

        return builder.Build();
    }

    private sealed class LifecycleSignals : ILoggerProvider
    {
        private readonly TaskCompletionSource _started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Started => _started.Task;

        public Task Stopped => _stopped.Task;

        public ILogger CreateLogger(string categoryName) => new LifecycleLogger(_started, _stopped);

        public void Dispose()
        {
        }

        private sealed class LifecycleLogger : ILogger
        {
            private readonly TaskCompletionSource _started;
            private readonly TaskCompletionSource _stopped;

            public LifecycleLogger(TaskCompletionSource started, TaskCompletionSource stopped)
            {
                _started = started;
                _stopped = stopped;
            }

            public IDisposable BeginScope<TState>(TState state)
                where TState : notnull => EmptyScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                var message = formatter(state, exception);

                if (message == "Migration agent started")
                {
                    _started.TrySetResult();
                }
                else if (message == "Migration agent stopped")
                {
                    _stopped.TrySetResult();
                }
            }
        }

        private sealed class EmptyScope : IDisposable
        {
            public static readonly EmptyScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
