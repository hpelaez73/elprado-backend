using Microsoft.AspNetCore.Http;

namespace ElPrado.Services.Interfaces
{
    public interface IImageStorageService
    {
        string SaveObituarioImage(int codObituario, IFormFile file);
        void DeleteObituarioImage(int codObituario, string imageUrl);
        bool Exists(int codObituario, string idImagen);
        byte[] GetObituarioImage(int codObituario, string idImagen);
    }
}
