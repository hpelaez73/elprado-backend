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
        public ActionResult<List<int>> PeriodosFacturacion()
        {
            try
            {
                List<int> listPeriodos = comprobantesService.PeriodosFacturacion();
                if (listPeriodos.Count == 0)
                {
                    return NotFound();
                }
                return listPeriodos;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.PeriodosFacturacion(): {Mensaje} {@Extras}", this, ex.Message);
                throw;
            }
        }

        [HttpGet("Facturas/{periodo}")]
        public IEnumerable<DtoComprobantesFacturasElectronicas> Facturas(int periodo)
        {
            try
            {
                return comprobantesService.Facturas(periodo);

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Facturas({periodo}): {Mensaje} {@Extras}", this, periodo, ex.Message);
                throw;
            }
        }
    }
}
