namespace Afip.Data.Models
{
    public class AfipTiposComprobantes
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int PuntoVenta { get; set; }
        public string Letra { get; set; } = string.Empty;
    }
}
