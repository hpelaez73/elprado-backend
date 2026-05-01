namespace ElPrado.Services.Workers
{
    /// <summary>
    /// Servicio para controlar el estado (pausado/activo) del EmailWorker
    /// </summary>
    public interface IWorkerStateService
    {
        bool IsPaused { get; }
        Task PauseAsync();
        Task ResumeAsync();
    }

    public class WorkerStateService : IWorkerStateService
    {
        private bool _isPaused = false;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public bool IsPaused
        {
            get => _isPaused;
        }

        public async Task PauseAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _isPaused = true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ResumeAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _isPaused = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
