using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class ConfiguracionGeneralRepository : RepositoryBaseEntidad<ConfiguracionGeneral>
    {
        public ConfiguracionGeneralRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public string BuscarMercadoPagoAccessToken()
        {
            return BuscarValorString(MacrosConfiguraciones.MercadoPagoAccessToken);
        }
        public string BuscarMercadoPagoBackUrlsSuccess()
        {
            return BuscarValorString(MacrosConfiguraciones.MercadoPagoBackUrlsSuccess);
        }
        public string BuscarMercadoPagoBackUrlsFailure()
        {
            return BuscarValorString(MacrosConfiguraciones.MercadoPagoBackUrlsFailure);
        }
        public string BuscarMercadoPagoBackUrlsPending()
        {
            return BuscarValorString(MacrosConfiguraciones.MercadoPagoBackUrlsPending);
        }
        public string BuscarMercadoPagoNotificationUrl()
        {
            return BuscarValorString(MacrosConfiguraciones.MercadoPagoNotificationUrl);
        }

        private string BuscarValorString(string macro)
        {
            List<ConfiguracionGeneral> listConfiguracionGeneral = conexion.GetList<ConfiguracionGeneral>(new { Macro = macro }, transaccion).AsList();
            return (listConfiguracionGeneral.Count == 0) ? string.Empty : listConfiguracionGeneral[0].ValorCaracter ?? string.Empty;
        }
    }

    internal static class MacrosConfiguraciones
    {
        public const string MercadoPagoAccessToken = "MP_ACC_TOK";
        public const string MercadoPagoBackUrlsSuccess = "MP_BU_SUCC";
        public const string MercadoPagoBackUrlsFailure = "MP_BU_FAIL";
        public const string MercadoPagoBackUrlsPending = "MP_BU_PEND";
        public const string MercadoPagoNotificationUrl = "MP_NOT_URL";
    }
}
