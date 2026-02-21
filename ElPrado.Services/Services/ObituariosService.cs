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
    public class ObituariosService : ServiceBaseCrud<Obituarios, DtoObituarios>
    {
        IImageStorageService? _imageStorageService;

        public ObituariosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override IMapper<Obituarios, DtoObituarios> CrearMapper()
        {
            return new ObituariosMapper();
        }

        protected override RepositoryBaseCrud<Obituarios, DtoObituarios> CrearRepository()
        {
            return _uow.Obituarios;
        }

        public Resultados<DtoObituarioDetalleResp> Detalle(int id)
        {
            DtoObituarioDetalleResp? obituarioDetalle = _uow.Obituarios.BuscarDetalle(id);
            if (obituarioDetalle != null)
            {
                obituarioDetalle.Condolencias = _uow.CondolenciasObituarios.BuscarPorObituario(id);
                obituarioDetalle.Comentarios = _uow.ComentariosObituarios.BuscarPorObituario(id);
                obituarioDetalle.Reacciones = _uow.Obituarios.BuscarReacciones(id);
                obituarioDetalle.Servicios = _uow.ServiciosObituarios.BuscarPorObituario(id);
            }

            Resultados<DtoObituarioDetalleResp> resultado = new()
            {
                Valor = obituarioDetalle
            };
            return resultado;
        }

        public Resultados AgregarReaccion(int codObituario, string tipoReaccion)
        {
            Resultados resultado = new();
            try
            {
                DtoDatabaseResp databaseResp = _uow.Obituarios.AgregarReaccion(codObituario, tipoReaccion);
                if (databaseResp.Resultado) _uow.Commit();
                else
                {
                    resultado.Agregar(databaseResp.MensajeError);
                    _uow.Rollback();
                }
            }
            catch { 
                _uow.Rollback();
                throw;
            }
            return resultado;
        }

        public void SetImageStorageService(IImageStorageService imageStorageService)
        {
            _imageStorageService = imageStorageService;
        }

        public Resultados GuardarImagen(int codObituario, IFormFile file)
        {
            Resultados resultado = new();
            if (!_uow.Obituarios.Existe(codObituario))
            {
                resultado.Agregar("El obituario no existe");
                return resultado;
            }
            if (_imageStorageService == null)
            {
                resultado.Agregar("El servicio de almacenamiento de imágenes no está configurado");
                return resultado;
            }

            Obituarios obituario = _uow.Obituarios.Buscar(codObituario);
            if (obituario == null)
            {
                resultado.Agregar("El obituario no existe");
                return resultado;
            }

            string oldImagen = string.Empty;

            if (!string.IsNullOrWhiteSpace(obituario.UrlImagen))
            {
                oldImagen = Path.GetFileName(obituario.UrlImagen);
            }

            string urlImagen = _imageStorageService.SaveObituarioImage(codObituario, file);

            try
            {
                _uow.Obituarios.ActualizarImagen(codObituario, urlImagen);
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                _imageStorageService.DeleteObituarioImage(codObituario, Path.GetFileName(urlImagen));

                throw;
            }

            // Borrar imagen anterior SOLO si DB fue exitosa
            if (!string.IsNullOrEmpty(oldImagen))
            {
                _imageStorageService.DeleteObituarioImage(codObituario, oldImagen);
            }

            return resultado;
        }

    }
}
