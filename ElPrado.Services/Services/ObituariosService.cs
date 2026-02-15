using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class ObituariosService : ServiceBaseCrud<Obituarios, DtoObituarios>
    {
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
                obituarioDetalle.Condolencias = _uow.Obituarios.BuscarCondolencias(id);
                obituarioDetalle.Comentarios = _uow.Obituarios.BuscarComentarios(id);
                obituarioDetalle.Reacciones = _uow.Obituarios.BuscarReacciones(id);
                obituarioDetalle.Servicios = _uow.Obituarios.BuscarServicios(id);
            }

            Resultados<DtoObituarioDetalleResp> resultado = new()
            {
                Valor = obituarioDetalle
            };
            return resultado;
        }
    }
}
