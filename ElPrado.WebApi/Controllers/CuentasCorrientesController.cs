using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class CuentasCorrientesController : ControladorBase
    {
        private CuentasCorrientesService cuentasCorrientesService => (_servicio as CuentasCorrientesService)!;

        public CuentasCorrientesController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new CuentasCorrientesService(_uow, _userContext);
        }

        [HttpGet("PendientesMercadoPago")]
        public ActionResult<ApiResponse<List<DtoCuentasCorrientes>>> PendientesMercadoPago()
        {
            ApiResponse<List<DtoCuentasCorrientes>> apiResponse = new();
            Resultados<List<DtoCuentasCorrientes>> resultado = cuentasCorrientesService.PendientesMercadoPago(_userContext.GetCodCliente());
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpGet("PendientesMercadoPago/{codCliente}")]
        public ActionResult<ApiResponse<List<DtoCuentasCorrientes>>> PendientesMercadoPago(int codCliente)
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

        [HttpPost("SolicitudMercadoPago")]
        public ActionResult<ApiResponse<string>> SolicitudMercadoPago([FromBody] List<DtoCuotasMercadoPago> listCuotas)
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

        [HttpPost("SolicitudMercadoPagoLink")]
        public ActionResult<ApiResponse<string>> SolicitudMercadoPagoLink([FromBody] DtoSolicitudMercadoPago dtoSolicitud)
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

        [AllowAnonymous]
        [HttpPost("NotificacionMercadoPago")]
        public ActionResult NotificacionMercadoPago(string topic, long id)
        {
            bool resultado = cuentasCorrientesService.NotificacionMercadoPago(topic, id);
            if (resultado) return Ok();
            else return BadRequest();
        }

        [HttpPost("ResumenCuentas")]
        public ActionResult<ApiResponse<List<DtoCuentasCorrientesResumen>>> ResumenCuentas([FromBody] DtoCuentasCorrientesResumenReq dtoCuentas)
        {
            ApiResponse<List<DtoCuentasCorrientesResumen>> apiResponse = new();
            Resultados<List<DtoCuentasCorrientesResumen>> resultado = cuentasCorrientesService.ResumenCuentas(dtoCuentas);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPost("ListadoResumenCuotas")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoResumenCuotas([FromBody] DtoOpcionesListados opcionesListado)
        {
            return cuentasCorrientesService.ListadoResumenCuotas(opcionesListado);
        }

    }
}
