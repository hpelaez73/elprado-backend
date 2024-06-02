using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ComprobantesController : ControladorBase
    {
        private ComprobantesService comprobantesService => (servicio as ComprobantesService)!;

        public ComprobantesController()
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new ComprobantesService(null);
        }

        [HttpGet("PeriodosFacturacion")]
        public ActionResult<ApiResponse<List<int>>> PeriodosFacturacion()
        {
            try
            {
                ApiResponse<List<int>> apiResponse = new();

                List<int> listPeriodos = comprobantesService.PeriodosFacturacion();
                if (listPeriodos.Count == 0)
                {
                    apiResponse.Agregar("No hay periodos facturados");
                    return NotFound(apiResponse);
                }
                apiResponse.Data = listPeriodos;
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.PeriodosFacturacion(): {Mensaje} {@Extras}", this, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpGet("Facturas/{periodo}")]
        public ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> Facturas(int periodo)
        {
            try
            {
                ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> apiResponse = new()
                {
                    Data = comprobantesService.Facturas(periodo)
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Facturas({periodo}): {Mensaje} {@Extras}", this, periodo, ex.Message, extrasLog);
                throw;
            }
        }

        #region Envio de facturas
        [HttpPost("ListadoFacturasEnviar")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoFacturasEnviar([FromBody] DtoOpcionesListados opcionesListado)
        {
            try
            {
                return comprobantesService.ListadoFacturasEnviar(opcionesListado);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.ListadoFacturasEnviar({@opcionesListado}): {Mensaje} {@Extras}", this, opcionesListado, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("RegistrarEnvio")]
        public ActionResult<ApiResponse<int>> RegistrarEnvio([FromBody] DtoRegistrarEnvioReq solicitud)
        {
            try
            {
                ApiResponse<int> apiResponse = new();
                Resultados resultado = comprobantesService.RegistrarEnvio(solicitud);
                if (resultado.HayError)
                {
                    apiResponse.Agregar(resultado);
                    return BadRequest(apiResponse);
                }
                apiResponse.Message = "Envio registrado";
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.RegistrarEnvio({@solicitud}): {Mensaje} {@Extras}", this, solicitud, ex.Message, extrasLog);
                throw;
            }
        }

        #endregion
    }
}
