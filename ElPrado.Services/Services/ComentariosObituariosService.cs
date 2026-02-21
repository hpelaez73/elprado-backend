using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Mappers;
using Microsoft.AspNetCore.Http;

namespace ElPrado.Services.Services
{
    public class ComentariosObituariosService : ServiceBaseCrud<ComentariosObituarios, DtoComentariosObituarios>
    {
        IImageStorageService? _imageStorageService;

        public ComentariosObituariosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public void SetImageStorageService(IImageStorageService imageStorageService)
        {
            _imageStorageService = imageStorageService;
        }

        protected override IMapper<ComentariosObituarios, DtoComentariosObituarios> CrearMapper()
        {
            return new ComentariosObituariosMapper();
        }

        protected override RepositoryBaseCrud<ComentariosObituarios, DtoComentariosObituarios> CrearRepository()
        {
            return _uow.ComentariosObituarios;
        }

        protected override Resultados ValidarAgregar(DtoComentariosObituarios dto)
        {
            Resultados resultado = new();
            
            Obituarios obituario = _uow.Obituarios.Buscar(dto.CodObituario);
            if (!obituario.PermitirComentarios)
            {
                resultado.Agregar("El obituario no permite comentarios");
            }
            dto.Aprobado = !obituario.ModerarComentarios;
            dto.Fecha = DateTime.Now;

            return resultado;
        }

        public Resultados GuardarImagen(int codComentario, int slot, IFormFile file)
        {
            Resultados resultado = new();
            if (_imageStorageService == null)
            {
                resultado.Agregar("El servicio de almacenamiento de imágenes no está configurado");
                return resultado;
            }

            if (slot < 1 || slot > 3)
            {
                resultado.Agregar("Slot de imagen inválido");
                return resultado;
            }

            ComentariosObituarios comentario = _uow.ComentariosObituarios.Buscar(codComentario);
            if (comentario == null)
            {
                resultado.Agregar("El comentario no existe");
                return resultado;
            }

            string oldImagen = slot switch
            {
                1 => Path.GetFileName(comentario.UrlImagen1),
                2 => Path.GetFileName(comentario.UrlImagen2),
                3 => Path.GetFileName(comentario.UrlImagen3),
                _ => string.Empty
            };

            string urlImagen = _imageStorageService.SaveObituarioImage(comentario.CodObituario, file);

            try
            {
                _uow.ComentariosObituarios.ActualizarImagen(codComentario, slot, urlImagen);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                _imageStorageService.DeleteObituarioImage(comentario.CodObituario, Path.GetFileName(urlImagen));

                throw;
            }

            // Borrar imagen anterior SOLO si DB fue exitosa
            if (!string.IsNullOrEmpty(oldImagen))
            {
                _imageStorageService.DeleteObituarioImage(comentario.CodObituario, oldImagen);
            }

            return resultado;
        }

    }
}
