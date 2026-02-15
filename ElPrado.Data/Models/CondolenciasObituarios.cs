namespace ElPrado.Data.Models
{
    public class CondolenciasObituarios : Entidades
    {
        [Key]
        public int CodCondolencia { get; set; }
        public int CodObituario { get; set; }
        public string Autor { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Aprobado { get; set; }
    }
}
