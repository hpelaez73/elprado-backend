using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class MediosCobrosService : ServiceBaseCrud<MediosCobros, DtoMediosCobros>
    {
        private MediosCobrosRepository mediosCobrosRepository => (repository as MediosCobrosRepository)!;

        public MediosCobrosService(Transaccion? transaccion) : base(transaccion)
        {
        }
        protected override RepositoryBaseCrud<MediosCobros, DtoMediosCobros> CrearRepositorio()
        {
            return new MediosCobrosRepository(Transaccion);
        }

        public List<DtoHistorialCobradores> BuscarHistorialCobradores(DtoCuentasCorrientesReq dtoCuentas)
        {
            int? codCredito = null;
            int? codConfiguracion = null;
            if (dtoCuentas.Tipo == "CR")
            {
                codCredito = dtoCuentas.Codigo;
            } 
            else if (dtoCuentas.Tipo == "CP")
            {
                codConfiguracion = dtoCuentas.Codigo;
            }
            else
            {
                codCredito = dtoCuentas.CodCredito == 0 ? null : dtoCuentas.CodCredito;
                codConfiguracion = dtoCuentas.CodConfiguracion == 0 ? null : dtoCuentas.CodConfiguracion;
            }
            return mediosCobrosRepository.BuscarHistorialCobradores(codCredito, codConfiguracion);
        }

    }
}
