namespace ElPrado.Services.Interfaces
{
    public interface IRecuperacionClaveEmailService
    {
        Task EnviarEnlaceAsync(string email, string token);
        Task EnviarAvisoAsync(string email);
    }
}
