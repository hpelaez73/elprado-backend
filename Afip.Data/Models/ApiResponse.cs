using ElPrado.Dto.Dtos;

namespace Afip.Data.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string NroCAE { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    // Compatibilidad con las operaciones existentes que no exponen datos tipados.
    public class ApiResponse : ApiResponse<object>
    {
    }
}
