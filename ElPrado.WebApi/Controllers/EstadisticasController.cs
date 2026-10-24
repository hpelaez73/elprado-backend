using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [Route("api/estadisticas/parcelas")]
    public sealed class EstadisticasController : ControladorBase
    {
        private EstadisticasService Estadisticas => (EstadisticasService)_servicio;
        public EstadisticasController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext) { }
        protected override ServiceBase CrearServicio() => new EstadisticasService(_uow, _userContext);

        /// <summary>Devuelve indicadores agregados del snapshot mensual de parcelas.</summary>
        [HttpGet("resumen")]
        [ProducesResponseType(typeof(ApiResponse<DtoEstadisticasResumen>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DtoEstadisticasResumen>), StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<DtoEstadisticasResumen>> Resumen([FromQuery] string periodo, [FromQuery] string? producto = null)
        {
            Resultados<DtoEstadisticasResumen> resultado = Estadisticas.Resumen(periodo, producto);
            ApiResponse<DtoEstadisticasResumen> respuesta = new();
            if (resultado.HayError) { respuesta.Agregar(resultado); return BadRequest(respuesta); }
            respuesta.Data = resultado.Valor;
            return Ok(respuesta);
        }

        /// <summary>Devuelve sólo observaciones disponibles y explicita períodos faltantes.</summary>
        [HttpGet("evolucion")]
        [ProducesResponseType(typeof(ApiResponse<DtoEstadisticasEvolucion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DtoEstadisticasEvolucion>), StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<DtoEstadisticasEvolucion>> Evolucion([FromQuery] string desde, [FromQuery] string hasta, [FromQuery] string? producto = null)
        {
            Resultados<DtoEstadisticasEvolucion> resultado = Estadisticas.Evolucion(desde, hasta, producto);
            ApiResponse<DtoEstadisticasEvolucion> respuesta = new();
            if (resultado.HayError) { respuesta.Agregar(resultado); return BadRequest(respuesta); }
            respuesta.Data = resultado.Valor;
            return Ok(respuesta);
        }
    }
}
