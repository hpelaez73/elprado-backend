namespace ElPrado.Migraciones.Agent.Workers;

public sealed class AgentWorker : BackgroundService
{
    private readonly ILogger<AgentWorker> _logger;

    public AgentWorker(ILogger<AgentWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Migration agent started");

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Expected when the host is stopping.
        }
        finally
        {
            _logger.LogInformation("Migration agent stopped");
        }
    }
}
