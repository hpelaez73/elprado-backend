using ElPrado.Data;
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
        public ApiResponse<List<DtoServiciosPropuesta>> ServiciosPropuesta(int codPropuesta)
        {
            ApiResponse<List<DtoServiciosPropuesta>> apiResponse = new()
            {
                Data = serviciosModelosService.ServiciosPropuesta(codPropuesta)
            };
            return apiResponse;
        }

        [HttpGet("ServiciosUtilizadosPropuesta/{codPropuesta}")]
        public ApiResponse<List<DtoServiciosUtilizadosPropuesta>> ServiciosUtilizadosPropuesta(int codPropuesta)
        {
            ApiResponse<List<DtoServiciosUtilizadosPropuesta>> apiResponse = new()
            {
                Data = serviciosModelosService.ServiciosUtilizadosPropuesta(codPropuesta)
            };
            return apiResponse;
        }

        [HttpGet("BeneficiariosPropuesta/{codPropuesta}")]
        public ApiResponse<List<DtoBeneficiariosPropuesta>> BeneficiariosPropuesta(int codPropuesta)
        {
            ApiResponse<List<DtoBeneficiariosPropuesta>> apiResponse = new()
            {
                Data = serviciosModelosService.BeneficiariosPropuesta(codPropuesta)
            };
            return apiResponse;
        }

    }
}
