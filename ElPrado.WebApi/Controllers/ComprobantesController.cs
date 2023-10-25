using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ComprobantesController : ControladorBase
    {
        private ComprobantesService comprobantesService;

        public ComprobantesController()
        {
            comprobantesService = new(null);
        }

        protected override void DisposeServicios()
        {
            comprobantesService.Dispose();
            base.DisposeServicios();
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
                Serilog.Log.Error(ex, "{Controlador}.PeriodosFacturacion(): {Mensaje} {@Extras}", this, ex.Message);
                throw;
            }
        }

        [HttpGet("Facturas/{periodo}")]
        public ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> Facturas(int periodo)
        {
            try
            {
                ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> apiResponse = new();
                apiResponse.Data = comprobantesService.Facturas(periodo);
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Facturas({periodo}): {Mensaje} {@Extras}", this, periodo, ex.Message);
                throw;
            }
        }
    }
}
