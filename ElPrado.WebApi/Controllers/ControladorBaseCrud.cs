using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorBaseCrud<TEntidad, TDto> : ControladorBase
        where TEntidad : Entidades, new()
        where TDto : DtoBase
    {
        protected ServiceBaseCrud<TEntidad, TDto> _serviceCrud => (_servicio as ServiceBaseCrud<TEntidad, TDto>)!;

        public ControladorBaseCrud(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return CrearServicioCrud();
        }

        protected virtual ServiceBaseCrud<TEntidad, TDto> CrearServicioCrud()
        {
            ServiceBaseCrud<TEntidad, TDto> serviceBaseCrud = new(_uow, _userContext);
            return serviceBaseCrud;
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<TDto>> Get(int id)
        {
            ApiResponse<TDto> apiResponse = new()
            {
                Data = _serviceCrud.Visualizar(id)
            };
            if (apiResponse.Data == null)
            {
                apiResponse.Agregar("El registro no existe");
                return NotFound(apiResponse);
            }
            return apiResponse;
        }

        [HttpPost]
        public ActionResult<ApiResponse<TDto>> Post([FromBody] TDto dto)
        {
            ApiResponse<TDto> apiResponse = new();
            Resultados<TDto> resultado = _serviceCrud.Agregar(dto);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPut]
        public ActionResult<ApiResponse<TDto>> Put([FromBody] TDto dto)
        {
            ApiResponse<TDto> apiResponse = new();
            Resultados<TDto> resultado = _serviceCrud.Modificar(dto);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<bool>> Delete(int id)
        {
            ApiResponse<bool> apiResponse = new();
            Resultados resultado = _serviceCrud.Eliminar(id);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = true;
            return apiResponse;
        }

        [HttpPost("Listado")]
        public ApiResponse<IEnumerable<dynamic>> Listado([FromBody] DtoOpcionesListados opcionesListado)
        {
            return _serviceCrud.Listado(opcionesListado);
        }
    }
}
