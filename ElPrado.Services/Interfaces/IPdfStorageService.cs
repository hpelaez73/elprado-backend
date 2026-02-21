namespace ElPrado.Services.Interfaces
{
    public interface IPdfStorageService
    {
        string GetFilePath(string fileName, string? year = null);
        bool Exists(string fileName, string? year = null);
        byte[] ReadFile(string fileName, string? year = null);
        Task<byte[]> ReadFileAsync(string fileName, string? year = null);
    }
}
