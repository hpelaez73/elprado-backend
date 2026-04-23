using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class MediosCobrosController : ControladorBaseCrud<MediosCobros, DtoMediosCobros>
    {
        private MediosCobrosService mediosCobrosService => (_servicio as MediosCobrosService)!;

        public MediosCobrosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBaseCrud<MediosCobros, DtoMediosCobros> CrearServicio()
        {
            return new MediosCobrosService(_uow, _userContext);
        }

        [HttpPost("HistorialCobradores")]
        public ApiResponse<List<DtoHistorialCobradores>> HistorialCobradores([FromBody] DtoCuentasCorrientesReq dtoCuentas)
        {
            ApiResponse<List<DtoHistorialCobradores>> apiResponse = new()
            {
                Data = mediosCobrosService.BuscarHistorialCobradores(dtoCuentas)
            };
            return apiResponse;
        }
    }
}
