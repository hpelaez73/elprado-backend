namespace ElPrado.Data.Models
{
    [Table("PROPUESTA")]
    public class Propuestas : Entidades
    {
        [Key]
        public int CodPropuesta { get; set; }
        public int Legajo { get; set; }
    }
}
