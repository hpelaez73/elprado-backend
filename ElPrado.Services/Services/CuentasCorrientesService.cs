using ElPrado.Core;
using ElPrado.Core.Utils;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class CuentasCorrientesService : ServiceBase
    {
        public CuentasCorrientesService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public Resultados<List<DtoCuentasCorrientes>> PendientesMercadoPago(int codCliente)
        {
            Resultados<List<DtoCuentasCorrientes>> resultado = new();

            DateOnly fechaDia = DateOnly.FromDateTime(DateTime.Today.AddMonths(-6).AddDays(1 - DateTime.Today.Day));
            List<DtoCuentasCorrientes> listCuotas = _uow.CuentasCorrientes.ConsultaDeudaMercadoPago(codCliente, fechaDia);

            if (_userContext.GetCodUsuario() == 0)
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

        public async Task<Resultados<string>> SolicitudMercadoPagoAsync(List<DtoCuotasMercadoPago> listCuotas)
        {
            Resultados<string> resultado = await GenerarSolicitudMercadoPagoAsync(listCuotas, _userContext.GetCodCliente(), null);

            return resultado;
        }

        public async Task<Resultados<string>> SolicitudMercadoPagoLinkAsync(DtoSolicitudMercadoPago dtoSolicitud)
        {
            Resultados<string> resultado = await GenerarSolicitudMercadoPagoAsync(dtoSolicitud.ListCuotas, dtoSolicitud.CodCliente, dtoSolicitud.VencimientoLink);

            return resultado;
        }

        private async Task<Resultados<string>> GenerarSolicitudMercadoPagoAsync(List<DtoCuotasMercadoPago> listCuotas, int codCliente, DateTime? fechaVencimiento)
        {
            Resultados<string> resultado = new();
            if (listCuotas.Count == 0) resultado.Agregar("No hay cuotas seleccionadas");

            if (_userContext.GetCodUsuario() != 0)
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
                if (listCuotasRestante.Exists(x => (!x.EsIndependiente && !item.EsIndependiente && x.Tipo == "CP" && item.Tipo == "CP" && x.Codigo == item.Codigo) 
                    || (x.CodGrupo == item.CodGrupo))) resultado.Agregar("Falta seleccionar cuotas del mismo grupo de cuenta");
            }

            if (listCuotasSolicitud.Sum(x => x.Total) == 0) resultado.Agregar("El importe no puede ser 0");

            if (resultado.HayError) return resultado;

            using MercadoPagoService mercadoPagoService = new(_uow, _userContext);

            resultado = await mercadoPagoService.ArmarPagoAsync(listCuotasSolicitud, codCliente, fechaVencimiento);
            RegistrarLog("Se generó una solicitud MP para cod_cliente: " + codCliente.ToString());

            if (resultado.EstaOK) _uow.Commit();

            return resultado;
        }

        public bool NotificacionMercadoPago(string topic, long id)
        {
            if (topic != "payment") return false;

            using MercadoPagoService mercadoPagoService = new(_uow, _userContext);
            mercadoPagoService.ImputarPago(id);
            _uow.Commit();

            return true;
        }

        public Resultados<List<DtoCuentasCorrientesResumen>> ResumenCuentas(DtoCuentasCorrientesResumenReq dtoCuentas)
        {
            Resultados<List<DtoCuentasCorrientesResumen>> resultado = new()
            {
                Valor = _uow.CuentasCorrientes.ResumenCuentas(
                    dtoCuentas.CodPropuesta, dtoCuentas.MostrarBaja, dtoCuentas.MostrarInactiva,
                    dtoCuentas.FechaInteres ?? DateUtils.FinDeMes(DateTime.Today.AddMonths(-1)),
                    dtoCuentas.FechaHasta ?? DateUtils.FinDeMes())
            };
            return resultado;
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoResumenCuotas(DtoOpcionesListados opcionesListado)
        {
            return _uow.CuentasCorrientes.ListadoResumenCuotas(opcionesListado);
        }
    }
}
