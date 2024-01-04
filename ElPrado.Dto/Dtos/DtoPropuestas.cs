namespace ElPrado.Dto.Dtos
{
    public class DtoPropuestas
    {
        public int CodPropuesta { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
    }
}
