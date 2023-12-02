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
        protected ServiceBaseCrud<TEntidad, TDto> serviceCrud => (servicio as ServiceBaseCrud<TEntidad, TDto>)!;

        public ControladorBaseCrud()
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return CrearServicioCrud();
        }

        protected virtual ServiceBaseCrud<TEntidad, TDto> CrearServicioCrud()
        {
            return new(null);
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<TDto>> Get(int id)
        {
            try
            {
                ApiResponse<TDto> apiResponse = new()
                {
                    Data = serviceCrud.Visualizar(id)
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
