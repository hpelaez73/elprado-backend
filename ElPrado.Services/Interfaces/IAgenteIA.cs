namespace ElPrado.Services.Interfaces
{
    public interface IAgenteIA
    {
        Task<string> GenerarTextoAsync(string prompt);
    }
}
