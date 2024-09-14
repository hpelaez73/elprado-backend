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

        public string BuscarInformacionCobranza1()
        {
            return BuscarValorString(MacrosConfiguraciones.InformacionCobranza1);
        }
        public string BuscarInformacionCobranza2()
        {
            return BuscarValorString(MacrosConfiguraciones.InformacionCobranza2);
        }
        public string BuscarInformacionCobranza3()
        {
            return BuscarValorString(MacrosConfiguraciones.InformacionCobranza3);
        }
        public string BuscarUrlFacturasPdf()
        {
            return BuscarValorString(MacrosConfiguraciones.UrlFacturasPdf);
        }

        public string BuscarClaveMaestra()
        {
            return BuscarValorString(MacrosConfiguraciones.ClaveMaestraWeb);
        }

        private string BuscarValorString(string macro)
        {
            List<ConfiguracionGeneral> listConfiguracionGeneral = conexion.GetList<ConfiguracionGeneral>(new { Macro = macro }, transaccion).AsList();
            return (listConfiguracionGeneral.Count == 0) ? string.Empty : listConfiguracionGeneral[0].ValorCaracter ?? string.Empty;
        }

    }

    internal static class MacrosConfiguraciones
    {
        // MercadoPago
        public const string MercadoPagoAccessToken = "MP_ACC_TOK";
        public const string MercadoPagoBackUrlsSuccess = "MP_BU_SUCC";
        public const string MercadoPagoBackUrlsFailure = "MP_BU_FAIL";
        public const string MercadoPagoBackUrlsPending = "MP_BU_PEND";
        public const string MercadoPagoNotificationUrl = "MP_NOT_URL";

        // Cobranza
        public const string InformacionCobranza1 = "INFO_COBR1";
        public const string InformacionCobranza2 = "INFO_COBR2";
        public const string InformacionCobranza3 = "INFO_COBR3";
        public const string UrlFacturasPdf = "URL_FACTUR";

        // Seguridad
        public const string ClaveMaestraWeb = "CLAVE_MWEB";

    }
}
