using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorBaseCrud<TEntidad, TDto> : ControladorBase
        where TEntidad : Entidades
        where TDto : DtoBase
    {
        protected ServiceBaseCrud<TEntidad, TDto> service;

        public ControladorBaseCrud()
        {
            service = CrearServicio();
        }

        protected virtual ServiceBaseCrud<TEntidad, TDto> CrearServicio()
        {
            return new(null);
        }

        protected override void DisposeServicios()
        {
            service.Dispose();
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<TDto>> Get(int id)
        {
            try
            {
                ApiResponse<TDto> apiResponse = new()
                {
                    Data = service.Visualizar(id)
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
                Serilog.Log.Error(ex, "{Controlador}.Get({id}): {Mensaje} {@Extras}", this, id, ex.Message);
                throw;
            }
        }
    }
}
