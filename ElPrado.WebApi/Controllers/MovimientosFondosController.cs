using ElPrado.Data;
using ElPrado.Dto;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class MovimientosFondosController : ControladorBase
    {
        private MovimientosFondosService movimientosFondosService => (_servicio as MovimientosFondosService)!;

        public MovimientosFondosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }
        protected override ServiceBase CrearServicio()
        {
            return new MovimientosFondosService(_uow, _userContext);
        }

        [HttpGet("{codMovimientoFondo}")]
        public ActionResult<ApiResponse<DtoMovimientosFondos>> Get(int codMovimientoFondo)
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
    }
}
