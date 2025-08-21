using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ComprobantesService : ServiceBase
    {
        public ComprobantesService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public DtoComprobantes? Visualizar(int codTalonario, string nroComprobante)
        {
            return _uow.Comprobantes.Visualizar(codTalonario, nroComprobante);
        }

        public IEnumerable<DtoComprobantesFacturasElectronicas> Facturas(int periodo)
        {
            return _uow.Comprobantes.Facturas(_userContext.GetCodCliente(), new DateTime(periodo, 1, 1), new DateTime(periodo, 12, 31));
        }

        public List<int> PeriodosFacturacion()
        {
            DtoComprobantesPeriodo? periodo = _uow.Comprobantes.PeriodosFacturacion(_userContext.GetCodCliente());
            if (periodo == null || periodo.MinFecha == DateOnly.MinValue)
            {
                return new();
            }
            else
            {
                List<int> listPeriodos = new();
                for (int i = periodo.MaxFecha.Year; i >= periodo.MinFecha.Year; i--)
                {
                    listPeriodos.Add(i);
                }
                return listPeriodos;
            }
        }


        #region Envio de facturas
        public ApiResponseListado<IEnumerable<dynamic>> ListadoFacturasEnviar(DtoOpcionesListados opcionesListado)
        {
            ApiResponseListado<IEnumerable<dynamic>> repListFacturas = _uow.Comprobantes.ListadoFacturasEnviar(opcionesListado);
            if (!opcionesListado.MostrarFiltros)
            {
                foreach (DtoFacturasEnviarList item in repListFacturas.Data!.Select(v => (DtoFacturasEnviarList)v))
                {
                    string strTelefono = Utils.ExtraerDigitos(item.TelefonoMovil);
                    if (strTelefono[0] == '0') { strTelefono = strTelefono[1..]; }
                    if (strTelefono.Length < 10) { strTelefono = "341" + strTelefono; }
                    if (strTelefono.Length > 10) { strTelefono = strTelefono.Remove(strTelefono.IndexOf("15"), 2); }
                    item.TelefonoMovil = strTelefono;

                    item.LinkWhatsApp = ArmarLinkWs(item.TelefonoMovil, item.Cliente, item.NroComprobante, item.Fecha, item.Propuesta);
                }
            }
            return repListFacturas;
        }

        private string ArmarLinkWs(string telefonoMovil, string cliente, string nroComprobante, DateOnly fecha, int propuesta)
        {
            string strTexto = $"Estimado/a%20*{cliente.Replace(" ", "%20")}*%20:%0A%0A" +
                $"Puede%20descargar%20su%20Factura%20Electronica%20desde:%20{_uow.ConfiguracionGeneral.BuscarUrlFacturasPdf()}/{fecha.Year}/{nroComprobante}-{propuesta.ToString().PadLeft(10, '0')}.pdf%0A%0A" +
                $"Ante%20cualquier%20duda%20contactenos%20a%20los%20siguientes%20numeros:%0A" +
                $"{_uow.ConfiguracionGeneral.BuscarInformacionCobranza1().Replace(" ", "%20")}%0A" +
                $"{_uow.ConfiguracionGeneral.BuscarInformacionCobranza2().Replace(" ", "%20")}%0A" +
                $"{_uow.ConfiguracionGeneral.BuscarInformacionCobranza3().Replace(" ", "%20")}";

            return $"https://api.whatsapp.com/send?phone=549{telefonoMovil}&text={strTexto}";
        }

        public Resultados RegistrarEnvio(DtoRegistrarEnvioReq solicitud)
        {
            Resultados resultado = new();
            if (solicitud.CodTalonario == 0 || solicitud.CodCliente == 0 || string.IsNullOrEmpty(solicitud.NroComprobante) || string.IsNullOrEmpty(solicitud.TelefonoMovil))
            {
                resultado.Agregar("Falta ingresar datos del envío");
                return resultado;
            }
            try
            {
                _uow.Comprobantes.RegistrarEnvio(solicitud.CodCliente, solicitud.CodTalonario, solicitud.NroComprobante, solicitud.TelefonoMovil, _userContext.GetCodUsuario());
                RegistrarLog("Envío de facturas a cod_cliente:" + solicitud.CodCliente.ToString());
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
            return resultado;
        }
        #endregion

        public ApiResponseListado<IEnumerable<dynamic>> ListadoComprobantesPropuesta(DtoOpcionesListados opcionesListado)
        {
            return _uow.Comprobantes.ListadoComprobantesPropuesta(opcionesListado);
        }

    }
}
