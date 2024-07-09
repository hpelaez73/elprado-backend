namespace ElPrado.Data.Models
{
    [Table("PARCELA")]
    public class Parcelas : Entidades
    {
        [Key]
        public int CodParcela { get; set; }
        public string Legajo { get; set; } = string.Empty;
    }
}
