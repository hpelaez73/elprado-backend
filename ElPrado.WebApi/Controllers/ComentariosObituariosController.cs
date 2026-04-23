using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ComentariosObituariosController : ControladorBaseCrud<ComentariosObituarios, DtoComentariosObituarios>
    {
        private ComentariosObituariosService comentariosObituariosService => (_servicio as ComentariosObituariosService)!;

        public ComentariosObituariosController(IUnitOfWork unitOfWork, IUserContextService userContext, IImageStorageService imageStorageService) : base(unitOfWork, userContext)
        {
            comentariosObituariosService.SetImageStorageService(imageStorageService);
        }

        protected override ServiceBaseCrud<ComentariosObituarios, DtoComentariosObituarios> CrearServicioCrud()
        {
            return new ComentariosObituariosService(_uow, _userContext);
        }

        [HttpPost("GuardarImagen/{id}/{slot}")]
        public ActionResult<ApiResponse<int>> GuardarImagen(int id, int slot, IFormFile file)
        {
            ApiResponse<int> apiResponse = new();

            if (file == null)
            {
                apiResponse.Agregar("Archivo requerido");
                return BadRequest(apiResponse);
            }

            Resultados resultado = comentariosObituariosService.GuardarImagen(id, slot, file);

            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            return apiResponse;
        }
    }
}
