using ElPrado.Dto.Dtos;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenteIAController : ControllerBase
    {
        private readonly AgenteIAService _agenteIAService;

        public AgenteIAController(Func<string, IAgenteIA> agenteFactory, IConfiguration config)
        {
            _agenteIAService = new AgenteIAService(agenteFactory, config);
        }


        [HttpPost("MejorarTexto")]
        public async Task<ActionResult<ApiResponse<DtoMejorarTextoResp>>> MejorarTexto([FromBody] DetMejorarTextoReq request)
        {
            ApiResponse<DtoMejorarTextoResp> apiResponse = new();
            if (string.IsNullOrWhiteSpace(request.Texto))
            {
                apiResponse.Agregar("El texto no puede estar vacío.");
                return BadRequest(apiResponse);
            }


            string textoMejorado = await _agenteIAService.MejorarTextoAsync(
                request.Texto,
                request.Contexto);

            if (string.IsNullOrWhiteSpace(textoMejorado))
            {
                apiResponse.Agregar("No se pudo mejorar el texto.");
                return BadRequest(apiResponse);
            }
            else
            {
                apiResponse.Data = new DtoMejorarTextoResp
                {
                    TextoMejorado = textoMejorado
                };
                return apiResponse;
            }
        }
    }
}
