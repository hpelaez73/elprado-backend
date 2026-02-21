using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ObituariosController : ControladorBaseCrud<Obituarios, DtoObituarios>
    {
        private ObituariosService obituariosService => (_servicio as ObituariosService)!;
        IImageStorageService _imageStorageService;

        public ObituariosController(IUnitOfWork unitOfWork, IUserContextService userContext, IImageStorageService imageStorageService) : base(unitOfWork, userContext)
        {
            _imageStorageService = imageStorageService;
            obituariosService.SetImageStorageService(imageStorageService);
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

        [HttpPost("AgregarReaccion")]
        public ActionResult<ApiResponse<int>> AgregarReaccion([FromBody] DtoAgregarReaccionReq dto)
        {
            ApiResponse<int> apiResponse = new();
            Resultados resultado = obituariosService.AgregarReaccion(dto.CodObituario, dto.TipoReaccion);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            return apiResponse;
        }

        [HttpPost("GuardarImagen/{id}")]
        public ActionResult<ApiResponse<int>> GuardarImagen(int id, IFormFile file)
        {
            ApiResponse<int> apiResponse = new();

            if (file == null)
            {
                apiResponse.Agregar("Archivo requerido");
                return BadRequest(apiResponse);
            }

            Resultados resultado = obituariosService.GuardarImagen(id, file);

            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            return apiResponse;
        }

        [AllowAnonymous]
        [HttpGet("Imagen/{id}/{idImagen}")]
        public IActionResult DescargarImagen(int id, string idImagen)
        {
            if (!_imageStorageService.Exists(id, idImagen))
                return NotFound("No se encuentra la imagen");

            var fileBytes = _imageStorageService.GetObituarioImage(id, idImagen, out string contentType);
            return File(fileBytes, contentType);
        }
    }
}
