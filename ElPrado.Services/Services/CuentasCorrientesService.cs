using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class CuentasCorrientesService : ServiceBase
    {
        private CuentasCorrientesRepository cuentasCorrientesRepository => (repository as CuentasCorrientesRepository)!;

        public CuentasCorrientesService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new CuentasCorrientesRepository(Transaccion);
        }

        public Resultados<List<DtoCuentasCorrientes>> PendientesMercadoPago(int codCliente)
        {
            Resultados<List<DtoCuentasCorrientes>> resultado = new();

            DateTime fechaDia = DateTime.Today.AddMonths(-6).AddDays(1 - DateTime.Today.Day);
            List<DtoCuentasCorrientes> listCuotas = cuentasCorrientesRepository.ConsultaDeudaMercadoPago(codCliente, fechaDia);

            if (ConfiguracionGeneralSesion.CodUsuario == 0)
            {
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
            }
            resultado.Valor = listCuotas;
            return resultado;
        }

        public Resultados<string> SolicitudMercadoPago(List<DtoCuotasMercadoPago> listCuotas)
        {
            Resultados<string> resultado = GenerarSolicitudMercadoPago(listCuotas, ConfiguracionGeneralSesion.CodCliente, null);

            return resultado;
        }

        public Resultados<string> SolicitudMercadoPagoLink(DtoSolicitudMercadoPago dtoSolicitud)
        {
            Resultados<string> resultado = GenerarSolicitudMercadoPago(dtoSolicitud.ListCuotas, dtoSolicitud.CodCliente, dtoSolicitud.VencimientoLink);

            return resultado;
        }

        private Resultados<string> GenerarSolicitudMercadoPago(List<DtoCuotasMercadoPago> listCuotas, int codCliente, DateTime? fechaVencimiento)
        {
            Resultados<string> resultado = new();
            if (listCuotas.Count == 0) resultado.Agregar("No hay cuotas seleccionadas");

            if (ConfiguracionGeneralSesion.CodUsuario != 0)
            {
                if (fechaVencimiento == null) resultado.Agregar("Falta ingresar la fecha de vencimiento del link");
                else if (fechaVencimiento <= DateTime.Today) resultado.Agregar("La fecha de vencimiento del link debe ser posterior a hoy");

                if (resultado.HayError) return resultado;
            }

            Resultados<List<DtoCuentasCorrientes>> resultadoPendientes = PendientesMercadoPago(codCliente);
            if (resultadoPendientes.HayError) resultado.Agregar(resultadoPendientes);
            if (resultadoPendientes.Valor == null || resultadoPendientes.Valor.Count == 0)
            {
                resultado.Agregar("No hay cuotas impagas");
                return resultado;
            }
            List<DtoCuentasCorrientes> listCuotasRestante = resultadoPendientes.Valor;
            List<DtoCuentasCorrientes> listCuotasSolicitud = new();
            foreach (DtoCuotasMercadoPago item in listCuotas)
            {
                DtoCuentasCorrientes? cuota = listCuotasRestante.FirstOrDefault(x => x.Tipo == item.Tipo && x.Codigo == item.Codigo && x.Cuota == item.Cuota && x.Pago == item.Pago);
                if (cuota == null)
                {
                    resultado.Agregar("Una cuota seleccionada no se encuentra entre las pendientes de pago");
                    break;
                }
                else
                {
                    listCuotasSolicitud.Add(cuota);
                    listCuotasRestante.Remove(cuota);
                }
            }

            // Verifico la integridad de las seleccionadas
            foreach (DtoCuentasCorrientes item in listCuotasSolicitud)
            {
                if (listCuotasRestante.Exists(x => (!x.EsIndependiente && !item.EsIndependiente && x.Tipo == "CP" && item.Tipo == "CP") || (x.CodGrupo == item.CodGrupo))) resultado.Agregar("Falta seleccionar cuotas del mismo grupo de cuenta");
            }

            if (listCuotasSolicitud.Sum(x => x.Total) == 0) resultado.Agregar("El importe no puede ser 0");

            if (resultado.HayError) return resultado;

            //            try
            //            {
            using MercadoPagoService mercadoPagoService = new(Transaccion);

            resultado = mercadoPagoService.ArmarPago(listCuotasSolicitud, codCliente, fechaVencimiento);
            RegistrarLog("Se generó una solicitud MP para cod_cliente: " + codCliente.ToString());

            if (resultado.EstaOK) Commit();
            else Rollback();
            //            }
            //            catch
            //            {
            //                Rollback();
            //                throw;
            //            }
            return resultado;
        }

        public bool NotificacionMercadoPago(string topic, long id)
        {
            if (topic != "payment") return false;

            //try
            //{
                using MercadoPagoService mercadoPagoService = new(Transaccion);

                mercadoPagoService.ImputarPago(id);

                Commit();
            //}
            //catch
            //{
                //Rollback();
                //throw;
            //}
            return true;
        }

    }
}
