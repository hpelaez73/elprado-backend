using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ObituariosController : ControladorBaseCrud<Obituarios, DtoObituarios>
    {
        private ObituariosService obituariosService => (_servicio as ObituariosService)!;

        public ObituariosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBaseCrud<Obituarios, DtoObituarios> CrearServicio()
        {
            return new ObituariosService(_uow, _userContext);
        }

        [HttpGet("Detalle/{id}")]
        public ActionResult<ApiResponse<DtoObituarioDetalleResp>> Detalle(int id)
        {
            ApiResponse<DtoObituarioDetalleResp> apiResponse = new();
            Resultados<DtoObituarioDetalleResp> resultado = obituariosService.Detalle(id);
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
