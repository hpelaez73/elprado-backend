using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class PropuestasController : ControladorBase
    {
        private PropuestasService propuestasService => (servicio as PropuestasService)!;

        public PropuestasController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new PropuestasService(null);
        }

        [HttpPost("Titulares")]
        public ActionResult<ApiResponse<List<DtoClientesPropuestas>>> Titulares([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            try
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
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Titulares({@dtoPropuesta}): {Mensaje} {@Extras}", this, dtoPropuesta, ex.Message, extrasLog);
                throw;
            }
        }

        [HttpPost("Detalle")]
        public ActionResult<ApiResponse<DtoPropuestaDetalleResp>> Detalle([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            try
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
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Detalle({@dtoPropuesta}): {Mensaje} {@Extras}", this, dtoPropuesta, ex.Message, extrasLog);
                throw;
            }

        }

        [HttpPost("DetalleContratos")]
        public ActionResult<ApiResponse<DtoPropuestaDetalleContratosResp>> DetalleContratos([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            try
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
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.DetalleContratos({@dtoPropuesta}): {Mensaje} {@Extras}", this, dtoPropuesta, ex.Message, extrasLog);
                throw;
            }

        }

        [HttpPost("DetalleHistorialTitulares")]
        public ActionResult<ApiResponse<List<DtoClientesPropuestasHistorial>>> DetalleHistorialTitulares([FromBody] DtoPropuestaDetalleReq dtoPropuesta)
        {
            try
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
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.DetalleHistorialTitulares({@dtoPropuesta}): {Mensaje} {@Extras}", this, dtoPropuesta, ex.Message, extrasLog);
                throw;
            }

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
