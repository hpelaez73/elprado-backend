using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ParcelasController : ControladorBase
    {
        private ParcelasService parcelasService => (_servicio as ParcelasService)!;

        public ParcelasController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new ParcelasService(_uow, _userContext);
        }

        [HttpPost("Coordenada")]
        public ActionResult<ApiResponse<DtoCoordenadas>> Coordenada([FromBody] DtoParcelasCoordenadasReq dtoParcela)
        {
            ApiResponse<DtoCoordenadas> apiResponse = new();
            Resultados<DtoCoordenadas> resultado = parcelasService.BuscarCoordenada(dtoParcela);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }
    }
}
