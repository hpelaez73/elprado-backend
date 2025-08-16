using ElPrado.Core.Domains;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class MediosCobrosService : ServiceBaseCrud<MediosCobros, DtoMediosCobros>
    {
        public MediosCobrosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
            _repositoryCrud = _uow.MediosCobros;
        }

        public List<DtoHistorialCobradores> BuscarHistorialCobradores(DtoCuentasCorrientesReq dtoCuentas)
        {
            int? codCredito = null;
            int? codConfiguracion = null;
            if (dtoCuentas.Tipo == TipoCuentaDomain.Credito)
            {
                codCredito = dtoCuentas.Codigo;
            }
            else if (dtoCuentas.Tipo == TipoCuentaDomain.CuotaPeriodica)
            {
                codConfiguracion = dtoCuentas.Codigo;
            }
            else
            {
                codCredito = dtoCuentas.CodCredito == 0 ? null : dtoCuentas.CodCredito;
                codConfiguracion = dtoCuentas.CodConfiguracion == 0 ? null : dtoCuentas.CodConfiguracion;
            }
            return _uow.MediosCobros.BuscarHistorialCobradores(codCredito, codConfiguracion);
        }

    }
}
