namespace ElPrado.Data.Models
{
    public class ProcesosSistemas : Entidades
    {
        [Key]
        public int CodProcesoSistema { get; set; }
        public string Proceso { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public bool ModuloWeb { get; set; }
    }
}
