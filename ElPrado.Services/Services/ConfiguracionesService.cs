using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class ConfiguracionesService : ServiceBase
    {
        public ConfiguracionesService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public void ActualizarPaginas(List<DtoProcesosSistemas> listProcesos)
        {
            try
            {
                _uow.ProcesosSistemas.LimpiarProcesosWeb();
                foreach (DtoProcesosSistemas item in listProcesos)
                {
                    _uow.ProcesosSistemas.AgregarModificar(ProcesosSistemasMapper.MapToEntidad(item), "Proceso");
                }
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }
    }
}
