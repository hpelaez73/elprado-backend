namespace ElPrado.Dto.Dtos
{
    public class DtoParcelas
    {
    }

    public class DtoParcelasCoordenadasReq
    {
        public int CodParcela { get; set; }
        public string? NroTelefono { get; set; }
    }

    public class DtoParcelasDetallesLugares
    {
        public int Nivel { get; set; }
        public int Lugares { get; set; }
        public string NombreInhumado1 { get; set; } = string.Empty;
        public string Estado1 { get; set; } = string.Empty;
        public string Color1 { get; set; } = "#000000";
        public string NombreInhumado2 { get; set; } = string.Empty;
        public string Estado2 { get; set; } = string.Empty;
        public string Color2 { get; set; } = "#000000";
        public string NombreInhumado3 { get; set; } = string.Empty;
        public string Estado3 { get; set; } = string.Empty;
        public string Color3 { get; set; } = "#000000";
        public string NombreInhumado4 { get; set; } = string.Empty;
        public string Estado4 { get; set; } = string.Empty;
        public string Color4 { get; set; } = "#000000";
        public string NombreInhumado5 { get; set; } = string.Empty;
        public string Estado5 { get; set; } = string.Empty;
        public string Color5 { get; set; } = "#000000";
        public string NombreInhumado6 { get; set; } = string.Empty;
        public string Estado6 { get; set; } = string.Empty;
        public string Color6 { get; set; } = "#000000";
    }
}
