using ElPrado.Core;
using ElPrado.Core.Domains;
using ElPrado.Data;
using ElPrado.Data.Models;
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

        public Resultados<string> SolicitudMercadoPago(List<DtoCuotasMercadoPago> listCuotas)
        {
            Resultados<string> resultado = new();
            if (listCuotas.Count == 0) resultado.Agregar("No hay cuotas seleccionadas");

            Resultados<List<DtoCuentasCorrientes>> resultadoPendientes = PendientesMercadoPago();
            if (resultadoPendientes.HayError) resultado.Agregar(resultadoPendientes);
            if (resultadoPendientes.Valor == null)
            {
                resultado.Agregar("No hay cuotas impagas");
                return resultado;
            }
            List<DtoCuentasCorrientes> listCuotasRestante = resultadoPendientes.Valor;
            List<DtoCuentasCorrientes> listCuotasSolicitud = new();
            foreach (DtoCuotasMercadoPago item in listCuotas)
            {
                DtoCuentasCorrientes? cuota = listCuotasRestante.First(x => x.Tipo == item.Tipo && x.Codigo == item.Codigo && x.Cuota == item.Cuota && x.Pago == item.Pago);
                if (cuota == null) resultado.Agregar("Una cuota seleccionada no se encuentra entre las pendientes de pago");
                else
                {
                    listCuotasSolicitud.Add(cuota);
                    listCuotasRestante.Remove(cuota);
                }
            }

            // Verifico la integridad de las seleccionadas
            foreach (DtoCuentasCorrientes item in listCuotasSolicitud)
            {
                if (listCuotasRestante.Exists(x => (!x.EsIndependiente && !item.EsIndependiente) || (x.CodGrupo == item.CodGrupo))) resultado.Agregar("Falta seleccionar cuotas del mismo grupo de cuenta");
            }

            if (listCuotasSolicitud.Sum(x => x.Total) == 0) resultado.Agregar("El importe no puede ser 0");

            //if (resultado.HayError) return resultado;

            try
            {
                using MercadoPagoService mercadoPagoService = new(transaccion);

                resultado = mercadoPagoService.ArmarPago(listCuotasSolicitud, null);

                if (resultado.EstaOK) transaccion.Commit();
                else transaccion.Rollback();
            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
            return resultado;
        }
    }
}
