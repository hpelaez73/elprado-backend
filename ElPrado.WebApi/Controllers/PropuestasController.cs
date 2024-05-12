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
                Serilog.Log.Error(ex, "{Controlador}.ListadoTitulares({@opcionesListado}): {Mensaje} {@Extras}", this, opcionesListado, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("ListadoInhumados")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoInhumados([FromBody] DtoOpcionesListados opcionesListado)
        {
            try
            {
                return propuestasService.ListadoInhumados(opcionesListado);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.ListadoInhumados({@opcionesListado}): {Mensaje} {@Extras}", this, opcionesListado, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("ListadoBeneficiarios")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoBeneficiarios([FromBody] DtoOpcionesListados opcionesListado)
        {
            try
            {
                return propuestasService.ListadoBeneficiarios(opcionesListado);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.ListadoBeneficiarios({@opcionesListado}): {Mensaje} {@Extras}", this, opcionesListado, ex.Message, extrasLog);
                throw;
            }
        }
    }
}
