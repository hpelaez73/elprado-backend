using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ComprobantesController : ControladorBase
    {
        private ComprobantesService comprobantesService => (_servicio as ComprobantesService)!;
        private readonly IPdfStorageService _pdfService;

        public ComprobantesController(IUnitOfWork unitOfWork, IUserContextService userContext, IPdfStorageService pdfService) : base(unitOfWork, userContext)
        {
            _pdfService = pdfService;
        }

        protected override ServiceBase CrearServicio()
        {
            return new ComprobantesService(_uow, _userContext);
        }

        [HttpPost("Visualizar")]
        public ActionResult<ApiResponse<DtoComprobantes>> Visualizar([FromBody] DtoComprobanteReq dtoComprobante)
        {
            ApiResponse<DtoComprobantes> apiResponse = new();
            DtoComprobantes? comprobante = comprobantesService.Visualizar(dtoComprobante.CodTalonario, dtoComprobante.NroComprobante);
            if (comprobante == null)
            {
                apiResponse.Agregar("No se encontró el comprobante");
                return NotFound(apiResponse);
            }
            apiResponse.Data = comprobante;
            return apiResponse;
        }

        [HttpGet("PeriodosFacturacion")]
        public ActionResult<ApiResponse<List<int>>> PeriodosFacturacion()
        {
            ApiResponse<List<int>> apiResponse = new();

            List<int> listPeriodos = comprobantesService.PeriodosFacturacion();
            if (listPeriodos.Count == 0)
            {
                apiResponse.Agregar("No hay periodos facturados");
                return NotFound(apiResponse);
            }
            apiResponse.Data = listPeriodos;
            return apiResponse;
        }

        [HttpGet("Facturas/{periodo}")]
        public ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> Facturas(int periodo)
        {
            ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> apiResponse = new()
            {
                Data = comprobantesService.Facturas(periodo)
            };
            return apiResponse;
        }

        [HttpGet("Factura/{year}/{fileName}")]
        public IActionResult DescargarFactura(string year, string fileName)
        {
            if (!_pdfService.Exists(fileName, year))
                return NotFound("Archivo no encontrado.");

            var fileBytes = _pdfService.ReadFile(fileName, year);
            return File(fileBytes, "application/pdf", fileName);
        }

        #region Envio de facturas
        [HttpPost("ListadoFacturasEnviar")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoFacturasEnviar([FromBody] DtoOpcionesListados opcionesListado)
        {
            return comprobantesService.ListadoFacturasEnviar(opcionesListado);
        }

        [HttpPost("RegistrarEnvio")]
        public ActionResult<ApiResponse<int>> RegistrarEnvio([FromBody] DtoRegistrarEnvioReq solicitud)
        {
            ApiResponse<int> apiResponse = new();
            Resultados resultado = comprobantesService.RegistrarEnvio(solicitud);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Message = "Envio registrado";
            return apiResponse;
        }
        #endregion

        [HttpPost("ListadoComprobantesPropuesta")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoComprobantesPropuesta([FromBody] DtoOpcionesListados opcionesListado)
        {
            return comprobantesService.ListadoComprobantesPropuesta(opcionesListado);
        }

    }
}
