using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class CuentasCorrientesController : ControladorBase
    {
        CuentasCorrientesService cuentasCorrientesService;

        public CuentasCorrientesController()
        {
            cuentasCorrientesService = new(null);
        }

        protected override void DisposeServicios()
        {
            cuentasCorrientesService.Dispose();
            base.DisposeServicios();
        }

        [HttpGet("PendientesMercadoPago")]
        public ActionResult<ApiResponse<List<DtoCuentasCorrientes>>> PendientesMercadoPago()
        {
            try
            {
                ApiResponse<List<DtoCuentasCorrientes>> apiResponse = new();
                Resultados<List<DtoCuentasCorrientes>> resultado = cuentasCorrientesService.PendientesMercadoPago();
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
                Serilog.Log.Error(ex, "{Controlador}.PendientesMercadoPago(): {Mensaje} {@Extras}", this, ex.Message);
                throw;
            }
        }

        [HttpPost("SolicitudMercadoPago")]
        public ActionResult<ApiResponse<string>> SolicitudMercadoPago([FromBody] List<DtoCuotasMercadoPago> listCuotas)
        {
            try
            {
                ApiResponse<string> apiResponse = new();
                Resultados<string> resultado = cuentasCorrientesService.SolicitudMercadoPago(listCuotas);
                if (resultado.HayError || string.IsNullOrEmpty(resultado.Valor))
                {
                    apiResponse.Agregar(resultado);
                    return BadRequest(apiResponse);
                }
                apiResponse.Data = resultado.Valor;
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.SolicitudMercadoPago({@listCuotas}): {Mensaje} {@Extras}", this, listCuotas, ex.Message);
                throw;
            }
        }
    }
}
