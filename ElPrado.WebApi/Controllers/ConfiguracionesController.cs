using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ConfiguracionesController : ControladorBase
    {
        private ConfiguracionesService configuracionesService => (_servicio as ConfiguracionesService)!;

        public ConfiguracionesController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new ConfiguracionesService(_uow, _userContext);
        }

        [HttpPost("Paginas")]
        public ActionResult<ApiResponse<int>> ActualizarPaginas([FromBody] List<DtoProcesosSistemas> listProcesos)
        {
            configuracionesService.ActualizarPaginas(listProcesos);
            ApiResponse<int> apiResponse = new()
            {
                Message = "Páginas actualizadas"
            };
            return Ok(apiResponse);
        }

    }
}
