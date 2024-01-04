namespace ElPrado.Data.Models
{
    public class MenusWeb
    {
        public int Nivel { get; set; }
        public int CodPadre { get; set; }
        public int Pos { get; set; }
        public int CodItemMenu { get; set; }
        public bool EsDivision { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Proceso { get; set; } = string.Empty;
        public string PathIcono { get; set; } = string.Empty;
        public List<MenusWeb>? Items { get; set; }
    }
}
