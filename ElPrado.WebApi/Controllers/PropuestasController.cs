using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class PropuestasController : ControladorBase
    {
        private PropuestasService propuestasService => (servicio as PropuestasService)!;

        public PropuestasController()
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new PropuestasService(null);
        }

        [HttpPost("ListadoTitulares")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoTitulares([FromBody] DtoOpcionesListados opcionesListado)
        {
            try
            {
                return propuestasService.ListadoTitulares(opcionesListado);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.ListadoTitulares({@opcionesListado}): {Mensaje} {@Extras}", this, opcionesListado, ex.Message);
                throw;
            }
        }

    }
}
