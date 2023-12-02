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
                Serilog.Log.Error(ex, "{Controlador}.Menu(): {Mensaje} {@Extras}", this, ex.Message);
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
                Serilog.Log.Error(ex, "{Controlador}.Panel(): {Mensaje} {@Extras}", this, ex.Message);
                throw;
            }
        }
    }
}
