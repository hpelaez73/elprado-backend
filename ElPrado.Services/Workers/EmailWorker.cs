using ElPrado.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElPrado.Services.Workers
{
    public class EmailWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly IWorkerStateService _workerStateService;

        public EmailWorker(IServiceProvider provider, IWorkerStateService workerStateService)
        {
            _provider = provider;
            _workerStateService = workerStateService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Si el worker está pausado, esperar sin procesar
                if (_workerStateService.IsPaused)
                {
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                using var scope = _provider.CreateScope();

                var procesador = scope.ServiceProvider
                    .GetRequiredService<ProcesadorEnviosService>();

                await procesador.ProcesarAsync();

                // Esperar 30s entre ciclos (RateLimiter controla la velocidad dentro del ciclo)
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
