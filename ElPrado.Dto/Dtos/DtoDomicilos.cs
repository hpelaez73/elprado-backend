namespace ElPrado.Dto.Dtos
{
    public class DtoDomicilos : DtoBase
    {
        public int CodDomicilio { get; set; }
        public string CodigoPostal { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public string Zona { get; set; } = string.Empty;
    }
}