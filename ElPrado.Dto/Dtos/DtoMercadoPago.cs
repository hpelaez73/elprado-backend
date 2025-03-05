namespace ElPrado.Dto.Dtos
{
    public class DtoMercadoPago
    {
        public int CodPreferencia { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string NotificationUrl { get; set; } = string.Empty;
        public string BackUrlsSuccess { get; set; } = string.Empty;
        public string BackUrlsFailure { get; set; } = string.Empty;
        public string BackUrlsPending { get; set; } = string.Empty;
    }

    public class DtoPreferencia
    {
        public string Id { get; set; } = string.Empty;
        public string InitPoint { get; set; } = string.Empty;
    }

    public class DtoPayment
    {
        public int CodPreferencia { get; set; }
        public bool PagoAprobado { get; set; }
        public string ReferenciaExterna { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
    }
}
