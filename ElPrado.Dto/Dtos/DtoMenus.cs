namespace ElPrado.Dto.Dtos
{
    public class DtoMenus
    {
        public bool EsUsuario { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public bool EsDivision { get; set; }
        public string Link { get; set; } = string.Empty;
        public List<DtoMenusItems>? Items { get; set; }
    }

    public class DtoMenusItems
    {
        public bool EsUsuario { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public bool EsDivision { get; set; }
        public string Link { get; set; } = string.Empty;
    }

    public class DtoPanel
    {
        public bool EsUsuario { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}

