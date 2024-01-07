using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class UsuariosController : ControladorBaseCrud<Usuarios, DtoUsuarios>
    {
        private UsuariosService usuariosService => (servicio as UsuariosService)!;

        public UsuariosController()
        {
        }

        protected override ServiceBaseCrud<Usuarios, DtoUsuarios> CrearServicio()
        {
            return new UsuariosService(null);
        }

        [HttpGet("Menu")]
        public ApiResponse<List<DtoMenus>> Menu()
        {
            try
            {
                ApiResponse<List<DtoMenus>> apiResponse = new()
                {
                    Data = usuariosService.BuscarMenu()
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Menu(): {Mensaje} {@Extras}", this, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpGet("Panel")]
        public ApiResponse<List<DtoPanel>> Panel()
        {
            try
            {
                ApiResponse<List<DtoPanel>> apiResponse = new()
                {
                    Data = usuariosService.BuscarPanel()
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Panel(): {Mensaje} {@Extras}", this, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("AnalizarSolicitudAutorizacion")]
        public ActionResult<ApiResponse<DtoAutorizacionesSolicitudResp>> AnalizarSolicitudAutorizacion([FromBody] DtoAutorizacionesSolicitudReq solicitud)
        {
            try
            {
                ApiResponse<DtoAutorizacionesSolicitudResp> apiResponse = new()
                {
                  //  Data = usuariosService.AnalizarSolicitudAutorizacion(solicitud)
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.AnalizarSolicitudAutorizacion({@solicitud}): {Mensaje} {@Extras}", this, solicitud, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("GenerarAutorizacion")]
        public ActionResult<ApiResponse<DtoAutorizacionesGeneracionResp>> GenerarAutorizacion([FromBody] DtoAutorizacionesGeneracionReq solicitud)
        {
            try
            {
                ApiResponse<DtoAutorizacionesGeneracionResp> apiResponse = new()
                {
                 //   Data = usuariosService.GenerarAutorizacion(solicitud)
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.GenerarAutorizacion({@solicitud}): {Mensaje} {@Extras}", this, solicitud, ex.Message, extrasLog);
                throw;
            }
        }
    }
}
