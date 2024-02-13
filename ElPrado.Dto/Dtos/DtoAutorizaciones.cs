namespace ElPrado.Dto.Dtos
{
    public class DtoAutorizacionesSolicitudReq
    {
        public string Parte1 { get; set; } = string.Empty;
        public string Parte2 { get; set; } = string.Empty;
        public string Parte3 { get; set; } = string.Empty;
    }

    public class DtoAutorizacionesSolicitudResp
    {
        public string Proceso { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
    }

    public class DtoAutorizacionesGeneracionReq
    {
        public string Parte1 { get; set; } = string.Empty;
        public string Parte2 { get; set; } = string.Empty;
        public string Parte3 { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }

    public class DtoAutorizacionesGeneracionResp
    {
        public string Parte1 { get; set; } = string.Empty;
        public string Parte2 { get; set; } = string.Empty;
        public string Parte3 { get; set; } = string.Empty;
    }
}
