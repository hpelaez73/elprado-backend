using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorBase : ControllerBase, IDisposable
    {
        private bool disposed;
        protected ServiceBase servicio; 

        public ControladorBase()
        {
            servicio = CrearServicio();
        }

        protected virtual ServiceBase CrearServicio()
        {
            return new(null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DisposeServicios();
            }
            disposed = true;
        }

        protected virtual void DisposeServicios()
        {
            servicio.Dispose();
        }

        [HttpPost("Listado")]
        public ApiResponse<IEnumerable<dynamic>> Listado([FromBody] DtoOpcionesListados opcionesListado)
        {
            try
            {
                ApiResponse<IEnumerable<dynamic>> apiResponse = new()
                {
                    Data = servicio.Listado(opcionesListado)
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Listado({@opcionesListado}): {Mensaje} {@Extras}", this, opcionesListado, ex.Message);
                throw;
            }
        }

        [HttpGet("Listado")]
        public ApiResponse<IEnumerable<dynamic>> Listado()
        {
            try
            {
                ApiResponse<IEnumerable<dynamic>> apiResponse = new()
                {
                    Data = servicio.Listado(null)
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Listado(): {Mensaje} {@Extras}", this, ex.Message);
                throw;
            }
        }

    }
}
