using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class CuentasCorrientesController : ControladorBase
    {
        private CuentasCorrientesService cuentasCorrientesService => (servicio as CuentasCorrientesService)!;

        public CuentasCorrientesController()
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new CuentasCorrientesService(null);
        }

        [HttpGet("PendientesMercadoPago")]
        public ActionResult<ApiResponse<List<DtoCuentasCorrientes>>> PendientesMercadoPago()
        {
            try
            {
                ApiResponse<List<DtoCuentasCorrientes>> apiResponse = new();
                Resultados<List<DtoCuentasCorrientes>> resultado = cuentasCorrientesService.PendientesMercadoPago(ConfiguracionGeneralSesion.CodCliente);
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
                Serilog.Log.Error(ex, "{Controlador}.PendientesMercadoPago(): {Mensaje} {@Extras}", this, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpGet("PendientesMercadoPago/{codCliente}")]
        public ActionResult<ApiResponse<List<DtoCuentasCorrientes>>> PendientesMercadoPago(int codCliente)
        {
            try
            {
                ApiResponse<List<DtoCuentasCorrientes>> apiResponse = new();
                Resultados<List<DtoCuentasCorrientes>> resultado = cuentasCorrientesService.PendientesMercadoPago(codCliente);
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
                Serilog.Log.Error(ex, "{Controlador}.PendientesMercadoPago({codCliente}): {Mensaje} {@Extras}", this, codCliente, ex.Message, extrasLog);
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
                Serilog.Log.Error(ex, "{Controlador}.SolicitudMercadoPago({@listCuotas}): {Mensaje} {@Extras}", this, listCuotas, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("SolicitudMercadoPagoLink")]
        public ActionResult<ApiResponse<string>> SolicitudMercadoPagoLink([FromBody] DtoSolicitudMercadoPago dtoSolicitud)
        {
            try
            {
                ApiResponse<string> apiResponse = new();
                Resultados<string> resultado = cuentasCorrientesService.SolicitudMercadoPagoLink(dtoSolicitud);
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
                Serilog.Log.Error(ex, "{Controlador}.SolicitudMercadoPagoLink({@dtoSolicitud}): {Mensaje} {@Extras}", this, dtoSolicitud, ex.Message, extrasLog);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("NotificacionMercadoPago")]
        public ActionResult NotificacionMercadoPago(string topic, long id)
        {
            try
            {
                bool resultado = cuentasCorrientesService.NotificacionMercadoPago(topic, id);
                if (resultado) return Ok();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.NotificacionMercadoPago({topic}, {id}): {Mensaje} {@Extras}", this, topic, id, ex.Message, extrasLog);
                throw;
            }
        }
    }
}
