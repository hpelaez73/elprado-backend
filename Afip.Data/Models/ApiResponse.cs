namespace Afip.Data.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string NroCAE { get; set; } = string.Empty;
    }
}
