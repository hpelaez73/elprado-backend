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
        public ActionResult<List<DtoCuentasCorrientes>> PendientesMercadoPago()
        {
            try
            {
                Resultados<List<DtoCuentasCorrientes>> resultado = cuentasCorrientesService.PendientesMercadoPago();
                if (resultado.HayError || resultado.Valor == null)
                {
                    return BadRequest(resultado);
                }
                return resultado.Valor;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.PendientesMercadoPago(): {Mensaje} {@Extras}", this, ex.Message);
                throw;
            }
        }
    }
}
