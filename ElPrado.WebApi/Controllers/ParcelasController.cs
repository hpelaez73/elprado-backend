using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ParcelasController : ControladorBase
    {
        private ParcelasService parcelasService => (servicio as ParcelasService)!;

        public ParcelasController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new ParcelasService(null);
        }

        [HttpPost("Coordenada")]
        public ActionResult<ApiResponse<DtoCoordenadas>> Coordenada([FromBody] DtoParcelasCoordenadasReq dtoParcela)
        {
            try
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
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Coordenada({@dtoParcela}): {Mensaje} {@Extras}", this, dtoParcela, ex.Message, extrasLog);
                throw;
            }

        }
    }
}
