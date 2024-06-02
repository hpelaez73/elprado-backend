using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using System.Collections.Generic;

namespace ElPrado.Services.Services
{
    public class ComprobantesService : ServiceBase
    {
        private ComprobantesRepository comprobantesRepository => (repository as ComprobantesRepository)!;
        private ConfiguracionGeneralRepository configuracionGeneralRepository;

        public ComprobantesService(Transaccion? transaccion) : base(transaccion)
        {
            configuracionGeneralRepository = new(Transaccion);
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new ComprobantesRepository(Transaccion);
        }

        public IEnumerable<DtoComprobantesFacturasElectronicas> Facturas(int periodo)
        {
            return comprobantesRepository.Facturas(ConfiguracionGeneralSesion.CodCliente, new DateTime(periodo, 1, 1), new DateTime(periodo, 12, 31));
        }

        public List<int> PeriodosFacturacion()
        {
            DtoComprobantesPeriodo? periodo = comprobantesRepository.PeriodosFacturacion(ConfiguracionGeneralSesion.CodCliente);
            if (periodo == null || periodo.MinFecha == DateTime.MinValue)
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
            ApiResponseListado<IEnumerable<dynamic>> repListFacturas = comprobantesRepository.ListadoFacturasEnviar(opcionesListado);
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

        private string ArmarLinkWs(string telefonoMovil, string cliente, string nroComprobante, DateTime fecha, int propuesta)
        {
            telefonoMovil = "3416953193";
            string strTexto = $"Estimado/a%20*{cliente.Replace(" ", "%20")}*%20:%0A%0A" +
                $"Puede%20descargar%20su%20Factura%20Electronica%20desde:%20{configuracionGeneralRepository.BuscarUrlFacturasPdf()}/{fecha.Year}/{nroComprobante}-{propuesta.ToString().PadLeft(10, '0')}.pdf%0A%0A"+
                $"Ante%20cualquier%20duda%20contactenos%20a%20los%20siguientes%20numeros:%0A" +
                $"{configuracionGeneralRepository.BuscarInformacionCobranza1().Replace(" ", "%20")}%0A" +
                $"{configuracionGeneralRepository.BuscarInformacionCobranza2().Replace(" ", "%20")}%0A" +
                $"{configuracionGeneralRepository.BuscarInformacionCobranza3().Replace(" ", "%20")}";

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
                comprobantesRepository.RegistrarEnvio(solicitud.CodCliente, solicitud.CodTalonario, solicitud.NroComprobante, solicitud.TelefonoMovil, ConfiguracionGeneralSesion.CodUsuario);
                Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            return resultado;
        }
        #endregion
    }
}
