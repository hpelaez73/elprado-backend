using ElPrado.Core.Enums;

namespace ElPrado.Data.Comun
{
    public static class TipoDatoListado
    {
        public const string Texto = "Texto";
        public const string Entero = "Entero";
        public const string Numero = "Numero";
        public const string Fecha = "Fecha";
        public const string Boolean = "Boolean";
    }

    public class ConfiguracionListado
    {
        public List<CamposListado> ListCampos { get; set; } = new();
    }

    public class CamposListado
    {
        public string Campo { get; set; } = string.Empty;
        public string Etiqueta { get; set; } = string.Empty;
        public string TipoDato { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public bool PermiteFiltrar { get; set; }
        public bool PermiteOrdenar { get; set; } = false;
        public string CampoSql { get; set; } = string.Empty;
        public string CampoSql2 { get; set; } = string.Empty;
        public int CampoSqlOrden { get; set; }
        public bool OrdenDefault { get; set; } = false;
        public bool AscendenteDefault { get; set; } = true;
        public bool FiltroDefault { get; set; }
        public TipoComparacion TipoComparacionDefault { get; set; } = TipoComparacion.Igual;
        public string ValorComparacionDefault { get; set; } = string.Empty;
    }

}
