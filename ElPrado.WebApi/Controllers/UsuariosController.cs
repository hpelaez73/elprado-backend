using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class UsuariosController : ControladorBaseCrud<Usuarios, DtoUsuarios>
    {
        private UsuariosService usuariosService => (_servicio as UsuariosService)!;

        public UsuariosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBaseCrud<Usuarios, DtoUsuarios> CrearServicio()
        {
            return new UsuariosService(_uow, _userContext);
        }

        [HttpGet("Menu")]
        public ApiResponse<List<DtoMenus>> Menu()
        {
            ApiResponse<List<DtoMenus>> apiResponse = new()
            {
                Data = usuariosService.BuscarMenu()
            };
            return apiResponse;
        }

        [HttpGet("Panel")]
        public ApiResponse<List<DtoPanel>> Panel()
        {
            ApiResponse<List<DtoPanel>> apiResponse = new()
            {
                Data = usuariosService.BuscarPanel()
            };
            return apiResponse;
        }

        [HttpPost("AnalizarSolicitudAutorizacion")]
        public ActionResult<ApiResponse<DtoAutorizacionesSolicitudResp>> AnalizarSolicitudAutorizacion([FromBody] DtoAutorizacionesSolicitudReq solicitud)
        {
            ApiResponse<DtoAutorizacionesSolicitudResp> apiResponse = new();
            Resultados<DtoAutorizacionesSolicitudResp> resultado = usuariosService.AnalizarSolicitudAutorizacion(solicitud);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPost("GenerarAutorizacion")]
        public ActionResult<ApiResponse<DtoAutorizacionesGeneracionResp>> GenerarAutorizacion([FromBody] DtoAutorizacionesGeneracionReq solicitud)
        {
            ApiResponse<DtoAutorizacionesGeneracionResp> apiResponse = new();
            Resultados<DtoAutorizacionesGeneracionResp> resultado = usuariosService.GenerarAutorizacion(solicitud);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }
    }
}
