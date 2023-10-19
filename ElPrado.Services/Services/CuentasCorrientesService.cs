using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class CuentasCorrientesService : ServiceBase
    {
        CreditosRepository creditosRepository;
        ConfigCuotasPeriodicasRepository configCuotasPeriodicasRepository;
        CuentasCorrientesRepository cuentasCorrientesRepository;

        public CuentasCorrientesService(Transaccion? transaccion) : base(transaccion)
        {
            creditosRepository = new(base.transaccion);
            configCuotasPeriodicasRepository = new(base.transaccion);
            cuentasCorrientesRepository = new(base.transaccion);
        }

        public Resultados<List<DtoCuentasCorrientes>> PendientesMercadoPago()
        {
            Resultados<List<DtoCuentasCorrientes>> resultado = new();

            DateTime fechaDia = DateTime.Today.AddMonths(-6).AddDays(1 - DateTime.Today.Day);
            List<DtoCuentasCorrientes> listCuotas = cuentasCorrientesRepository.ConsultaDeudaMercadoPago(ConfiguracionGeneralSesion.CodCliente, fechaDia);

            if (listCuotas != null && listCuotas.Count > 0)
            {
                if (listCuotas.Exists(x => !x.MedioCobroHabilitado))
                {
                    resultado.Agregar("Algunas cuotas no están habilitadas para pagar por este medio. Consulte a la administración");
                }
                if (listCuotas.Exists(x => x.ConDeudaGrande))
                {
                    resultado.Agregar("Algunas cuotas superan la fecha permitida para pagar por este medio. Consulte a la administración");
                }
            }
            resultado.Valor = listCuotas;
            return resultado;
        }
    }
}
