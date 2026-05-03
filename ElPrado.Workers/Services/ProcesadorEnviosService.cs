using ElPrado.DataFactory.Interfaces;
using ElPrado.Workers.Interfaces;

namespace ElPrado.Workers.Services
{
    public class ProcesadorEnviosService
    {
        private readonly IColaEnvioFacturaRepository _repo;
        private readonly IEmailService _email;
        private static readonly SemaphoreSlim _throttle = new(10, 10); // Máximo 10 emails concurrentes
        private static DateTime _lastEmailTime = DateTime.MinValue;
        private static readonly TimeSpan _minIntervalBetweenEmails = TimeSpan.FromSeconds(1); // Mínimo 1 segundo entre emails

        public ProcesadorEnviosService(IColaEnvioFacturaRepository repo, IEmailService email)
        {
            _repo = repo;
            _email = email;
        }

        public async Task ProcesarAsync()
        {
            var listPendientes = await _repo.BuscarPendientesAsync();

            // Limitar a 5 concurrentes por ciclo
            var tasks = new List<Task>();
            var maxConcurrency = 5;

            foreach (var factura in listPendientes.Take(maxConcurrency))
            {
                tasks.Add(ProcesarFacturaAsync(factura));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcesarFacturaAsync(dynamic factura)
        {
            try
            {
                // Esperar a que haya slot disponible (máx 10 concurrentes)
                await _throttle.WaitAsync();

                try
                {
                    // Respetar intervalo mínimo entre emails
                    var timeSinceLastEmail = DateTime.UtcNow - _lastEmailTime;
                    if (timeSinceLastEmail < _minIntervalBetweenEmails)
                    {
                        await Task.Delay(_minIntervalBetweenEmails - timeSinceLastEmail);
                    }

                    await _repo.MarcarEnviandoAsync(factura.CodColaEnvio);

                    // Timeout de 30 segundos para evitar bloqueos indefinidos
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                    {
                        var task = _email.EnviarFacturaAsync(factura);
                        await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));

                        if (!task.IsCompleted)
                        {
                            throw new TimeoutException("El envío de email excedió 30 segundos");
                        }

                        await task;
                    }

                    _lastEmailTime = DateTime.UtcNow;
                    await _repo.MarcarEnviadoAsync(factura.CodColaEnvio);
                }
                finally
                {
                    _throttle.Release();
                }
            }
            catch (Exception ex)
            {
                await _repo.MarcarErrorAsync(factura.CodColaEnvio, ex.Message);
            }
        }
    }
}
