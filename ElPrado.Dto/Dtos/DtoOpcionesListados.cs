using ElPrado.Core.Enums;

namespace ElPrado.Dto.Dtos
{
    public class DtoOpcionesListados
    {
        public bool MostrarFiltros { get; set; }
        public int Pagina { get; set; }
        public int FilasPagina { get; set; } = 10;
        public bool SinPaginado { get; set; } = false;
        public List<DtoCamposFiltroListado>? ListFiltros { get; set; }
        public List<DtoCamposOrdenListado>? ListOrden { get; set; }
    }

    public class DtoCamposFiltroListado
    {
        public string Campo { get; set; } = string.Empty;
        public TipoComparacion TipoComparacion { get; set; }
        public string Valor { get; set; } = string.Empty;
        public string ValorHasta { get; set; } = string.Empty;
        public List<string>? ListValores { get; set; }
    }

    public class DtoCamposOrdenListado
    {
        public string Campo { get; set; } = string.Empty;
        public bool Ascendente { get; set; }
    }

    public class DtoConfiguracionListado
    {
        public List<DtoCamposListado> ListCampos { get; set; } = new();
    }

    public class DtoCamposListado
    {
        public string Campo { get; set; } = string.Empty;
        public string Etiqueta { get; set; } = string.Empty;
        public string TipoDato { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public bool PermiteFiltrar { get; set; }
        public bool PermiteOrdenar { get; set; }
    }
}
