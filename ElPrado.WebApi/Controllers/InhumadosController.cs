using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class InhumadosController : ControladorBase
    {
        private InhumadosService inhumadosService => (_servicio as InhumadosService)!;

        public InhumadosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new InhumadosService(_uow, _userContext);
        }

        [HttpGet("Inhumacion/{id}")]
        public ActionResult<ApiResponse<DtoInhumacion>> Inhumacion(int id)
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

        [AllowAnonymous]
        [HttpGet("ServiciosEnCurso")]
        public ActionResult<ApiResponse<List<DtoServiciosInhumacionesResp>>> ServiciosEnCurso()
        {
            ApiResponse<List<DtoServiciosInhumacionesResp>> apiResponse = new()
            {
                Data = inhumadosService.ServiciosEnCurso()
            };
            if (apiResponse.Data == null || apiResponse.Data.Count == 0)
            {
                apiResponse.Agregar("No hay servicios en curso");
                return NotFound(apiResponse);
            }
            return apiResponse;
        }

    }
}
