using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class ConfiguracionesService : ServiceBase
    {
        public ConfiguracionesService(Transaccion? transaccion) : base(transaccion)
        {
        }
        protected override RepositoryBase CrearRepositorio()
        {
            return new RepositoryBase(Transaccion);
        }

        public void ActualizarPaginas(List<DtoProcesosSistemas> listProcesos)
        {
            ProcesosSistemasRepository procesosSistemasRepository = new(Transaccion);
            try
            {
                procesosSistemasRepository.LimpiarProcesosWeb();
                foreach (DtoProcesosSistemas item in listProcesos)
                {
                    procesosSistemasRepository.AgregarModificar(ProcesosSistemasMapper.MapToEntidad(item), "Proceso");
                }
                Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
        }
    }
}
