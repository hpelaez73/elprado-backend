using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ConfiguracionesController : ControladorBase
    {
        private ConfiguracionesService configuracionesService => (servicio as ConfiguracionesService)!;

        public ConfiguracionesController()
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new ConfiguracionesService(null);
        }

        [HttpPost("Paginas")]
        public ActionResult<ApiResponse<int>> ActualizarPaginas([FromBody] List<DtoProcesosSistemas> listProcesos)
        {
            try
            {
                configuracionesService.ActualizarPaginas(listProcesos);
                ApiResponse<int> apiResponse = new()
                {
                    Message = "Páginas actualizadas"
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.ActualizarPaginas({@listProcesos}): {Mensaje} {@Extras}", this, listProcesos, ex.Message, extrasLog);
                throw;
            }
        }

    }
}
