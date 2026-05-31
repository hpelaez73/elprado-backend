using ElPrado.Core.Enums;

namespace ElPrado.Dto.Dtos
{
    public class DetMejorarTextoReq
    {
        public string Texto { get; set; } = string.Empty;
        public ContextoIA Contexto { get; set; }
    }

    public class DtoMejorarTextoResp
    {
        public string TextoMejorado { get; set; } = string.Empty;
    }
}
