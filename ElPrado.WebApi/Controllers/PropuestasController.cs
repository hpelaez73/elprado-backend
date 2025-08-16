using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class PropuestasController : ControladorBase
    {
        private PropuestasService propuestasService => (_servicio as PropuestasService)!;

        public PropuestasController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new PropuestasService(_uow, _userContext);
        }

        [HttpPost("Titulares")]
        public ActionResult<ApiResponse<List<DtoClientesPropuestas>>> Titulares([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            ApiResponse<List<DtoClientesPropuestas>> apiResponse = new();
            Resultados<List<DtoClientesPropuestas>> resultado = propuestasService.Titulares(dtoPropuesta);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPost("Detalle")]
        public ActionResult<ApiResponse<DtoPropuestaDetalleResp>> Detalle([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            ApiResponse<DtoPropuestaDetalleResp> apiResponse = new();
            Resultados<DtoPropuestaDetalleResp> resultado = propuestasService.Detalle(dtoPropuesta);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPost("DetalleContratos")]
        public ActionResult<ApiResponse<DtoPropuestaDetalleContratosResp>> DetalleContratos([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            ApiResponse<DtoPropuestaDetalleContratosResp> apiResponse = new();
            Resultados<DtoPropuestaDetalleContratosResp> resultado = propuestasService.DetalleContratos(dtoPropuesta);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPost("DetalleHistorialTitulares")]
        public ActionResult<ApiResponse<List<DtoClientesPropuestasHistorial>>> DetalleHistorialTitulares([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            ApiResponse<List<DtoClientesPropuestasHistorial>> apiResponse = new();
            Resultados<List<DtoClientesPropuestasHistorial>> resultado = propuestasService.DetalleHistorialTitulares(dtoPropuesta);
            if (resultado.HayError || resultado.Valor == null)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = resultado.Valor;
            return apiResponse;
        }

        [HttpPost("ListadoTitulares")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoTitulares([FromBody] DtoOpcionesListados opcionesListado)
        {
            return propuestasService.ListadoTitulares(opcionesListado);
        }

        [HttpPost("ListadoInhumados")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoInhumados([FromBody] DtoOpcionesListados opcionesListado)
        {
            return propuestasService.ListadoInhumados(opcionesListado);
        }

        [HttpPost("ListadoBeneficiarios")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoBeneficiarios([FromBody] DtoOpcionesListados opcionesListado)
        {
            return propuestasService.ListadoBeneficiarios(opcionesListado);
        }
    }
}
