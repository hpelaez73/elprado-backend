using ElPrado.Data;
using ElPrado.Dto;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class MovimientosFondosController : ControladorBase
    {
        private MovimientosFondosService movimientosFondosService => (servicio as MovimientosFondosService)!;

        public MovimientosFondosController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        protected override ServiceBase CrearServicio()
        {
            return new MovimientosFondosService(null);
        }

        [HttpGet("{codMovimientoFondo}")]
        public ActionResult<ApiResponse<DtoMovimientosFondos>> Get(int codMovimientoFondo)
        {
            try
            {
                ApiResponse<DtoMovimientosFondos> apiResponse = new();
                DtoMovimientosFondos? movimientoFondo = movimientosFondosService.Visualizar(codMovimientoFondo);
                if (movimientoFondo == null)
                {
                    apiResponse.Agregar("No se encontró el movimiento de fondo");
                    return NotFound(apiResponse);
                }
                apiResponse.Data = movimientoFondo;
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Get({codMovimientoFondo}): {Mensaje} {@Extras}", this, codMovimientoFondo, ex.Message, extrasLog);
                throw;
            }
        }
    }
}
