using ElPrado.Dto.Configuration;
using ElPrado.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IO;
using System.Security.Policy;

namespace ElPrado.Services.Services
{
    public class LocalImageStorageService : IImageStorageService
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public LocalImageStorageService(IOptions<ImagenSettings> options)
        {
            _basePath = Path.GetFullPath(options.Value.BasePath);
            _baseUrl = options.Value.BaseUrl;

            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public bool Exists(int codObituario, string idImagen)
        {
            string folderPath = Path.Combine(_basePath, "obituarios", codObituario.ToString());
            string fullPath = Path.Combine(folderPath, idImagen);

            return File.Exists(fullPath);
        }

        public string SaveObituarioImage(int codObituario, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo inválido");

            string folderPath = Path.Combine(_basePath, "obituarios", codObituario.ToString());

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string apiUrlPath = $"{_baseUrl}/Obituarios/Imagen/{codObituario}/{fileName}";
            return apiUrlPath;
        }

        public void DeleteObituarioImage(int codObituario, string imageUrl)
        {
            string folderPath = Path.Combine(_basePath, "obituarios", codObituario.ToString());
            string fullPath = Path.Combine(folderPath, imageUrl);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public byte[] GetObituarioImage(int codObituario, string idImagen)
        {
            throw new NotImplementedException();
        }
    }
}

