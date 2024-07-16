using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class InhumadosController : ControladorBase
    {
        private InhumadosService inhumadosService => (servicio as InhumadosService)!;

        public InhumadosController()
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new InhumadosService(null);
        }

        [HttpGet("Inhumacion/{id}")]
        public ActionResult<ApiResponse<DtoInhumacion>> Inhumacion(int id)
        {
            try
            {
                ApiResponse<DtoInhumacion> apiResponse = new()
                {
                    Data = inhumadosService.BuscarInhumacion(id)
                };
                if (apiResponse.Data == null)
                {
                    apiResponse.Agregar("El registro no existe");
                    return NotFound(apiResponse);
                }
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Inhumacion({id}): {Mensaje} {@Extras}", this, id, ex.Message, extrasLog);
                throw;
            }
        }
    }
}
