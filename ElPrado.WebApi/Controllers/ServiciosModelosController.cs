using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ServiciosModelosController : ControladorBase
    {
        private ServiciosModelosService serviciosModelosService => (_servicio as ServiciosModelosService)!;

        public ServiciosModelosController(IUnitOfWork unitOfWork, IUserContextService userContextService) : base(unitOfWork, userContextService)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new ServiciosModelosService(_uow, _userContext);
        }

        [HttpGet("ServiciosPropuesta/{codPropuesta}")]
        public ActionResult<ApiResponse<DtoServiciosPropuesta>> ServiciosPropuesta(int codPropuesta)
        {
            ApiResponse<DtoServiciosPropuesta> apiResponse = new()
            {
                Data = serviciosModelosService.ServiciosPropuesta(codPropuesta)
            };
            return apiResponse;
        }
    }
}
