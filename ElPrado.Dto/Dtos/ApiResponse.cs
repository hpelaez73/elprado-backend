using ElPrado.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ElPrado.Dto.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } 
        public T? Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Success = true;
            Message = string.Empty;
            Errors = new List<string>();
        }

        public void Agregar(Resultados resultado)
        {
            if (resultado.HayError)
            {
                Success = false;
                Errors.AddRange(resultado.Errores);
            }
        }

        public void Agregar(string mensajeError)
        {
            Success = false;
            Errors.Add(mensajeError);
        }
    }
}
