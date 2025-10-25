using ElPrado.Dto.Configuration;
using Microsoft.Extensions.Options;

namespace ElPrado.Services.Services
{
    public interface IPdfStorageService
    {
        string GetFilePath(string fileName, string? year = null);
        bool Exists(string fileName, string? year = null);
        byte[] ReadFile(string fileName, string? year = null);
        Task<byte[]> ReadFileAsync(string fileName, string? year = null);
    }

    public class PdfStorageService : IPdfStorageService
    {
        private readonly string _basePath;

        public PdfStorageService(IOptions<PdfSettings> options)
        {
            _basePath = Path.GetFullPath(options.Value.BasePath);

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public string GetFilePath(string fileName, string? year = null)
        {
            // Asegura que el nombre del archivo no contenga rutas maliciosas
            var safeName = Path.GetFileName(fileName);

            string path = _basePath;

            if (!string.IsNullOrWhiteSpace(year))
            {
                // Asegura que el año sea un número de 4 dígitos
                if (!int.TryParse(year, out var y) || y < 2000 || y > 2100)
                    throw new ArgumentException("Año inválido");

                path = Path.Combine(_basePath, year);
            }

            return Path.Combine(path, safeName);
        }

        public bool Exists(string fileName, string? year = null)
        {
            var path = GetFilePath(fileName, year);
            return File.Exists(path);
        }

        public byte[] ReadFile(string fileName, string? year = null)
        {
            var path = GetFilePath(fileName, year);
            if (!File.Exists(path))
                throw new FileNotFoundException("PDF no encontrado", fileName);

            return File.ReadAllBytes(path);
        }

        public async Task<byte[]> ReadFileAsync(string fileName, string? year = null)
        {
            var path = GetFilePath(fileName, year);
            if (!File.Exists(path))
                throw new FileNotFoundException("PDF no encontrado", fileName);

            return await File.ReadAllBytesAsync(path);
        }
    }
}
