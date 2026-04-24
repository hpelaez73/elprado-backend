using ElPrado.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElPrado.Services.Workers
{
    public class EmailWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public EmailWorker(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _provider.CreateScope();

                var procesador = scope.ServiceProvider
                    .GetRequiredService<ProcesadorEnviosService>();

                await procesador.ProcesarAsync();

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
