using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Documents;
using ElPrado.Dto.Dtos;
using ElPrado.Reports.Interfaces;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class ComprobantesController : ControladorBase
    {
        private ComprobantesService comprobantesService => (_servicio as ComprobantesService)!;
        private readonly IPdfStorageService _pdfStorageService;
        private readonly IPdfService _pdfService;
        private readonly IReportImageService _reportImageService;

        public ComprobantesController(IUnitOfWork unitOfWork, IUserContextService userContext, IPdfStorageService pdfStorageService, IPdfService pdfService, IReportImageService reportImageService) : base(unitOfWork, userContext)
        {
            _pdfStorageService = pdfStorageService;
            _pdfService = pdfService;
            _reportImageService = reportImageService;
        }

        protected override ServiceBase CrearServicio()
        {
            return new ComprobantesService(_uow, _userContext);
        }

        [HttpPost("Visualizar")]
        public async Task<ActionResult<ApiResponse<DtoComprobantes>>> VisualizarAsync([FromBody] DtoComprobanteReq dtoComprobante)
        {
            ApiResponse<DtoComprobantes> apiResponse = new();
            DtoComprobantes? comprobante = await comprobantesService.VisualizarAsync(dtoComprobante.CodTalonario, dtoComprobante.NroComprobante);
            if (comprobante == null)
            {
                apiResponse.Agregar("No se encontró el comprobante");
                return NotFound(apiResponse);
            }
            apiResponse.Data = comprobante;
            return apiResponse;
        }

        [HttpGet("PeriodosFacturacion")]
        public async Task<ActionResult<ApiResponse<List<int>>>> PeriodosFacturacionAsync()
        {
            ApiResponse<List<int>> apiResponse = new();

            List<int> listPeriodos = await comprobantesService.PeriodosFacturacionAsync();
            if (listPeriodos.Count == 0)
            {
                apiResponse.Agregar("No hay periodos facturados");
                return NotFound(apiResponse);
            }
            apiResponse.Data = listPeriodos;
            return apiResponse;
        }

        [HttpGet("Facturas/{periodo}")]
        public async Task<ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>>> FacturasAsync(int periodo)
        {
            ApiResponse<IEnumerable<DtoComprobantesFacturasElectronicas>> apiResponse = new()
            {
                Data = await comprobantesService.FacturasAsync(periodo)
            };
            return apiResponse;
        }

        [HttpGet("Factura/{year}/{fileName}")]
        public IActionResult DescargarFactura(string year, string fileName)
        {
            if (!_pdfStorageService.Exists(fileName, year))
                return NotFound("Archivo no encontrado.");

            var fileBytes = _pdfStorageService.ReadFile(fileName, year);
            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpPost("Factura")]
        public async Task<IActionResult> DescargarFacturaAsync([FromBody] DtoComprobanteReq dtoComprobante)
        {
            DocFactura? factura = await comprobantesService.FacturaPdfAsync(dtoComprobante.CodTalonario, dtoComprobante.NroComprobante);

            if (factura == null)
            {
                return NotFound("Archivo no encontrado.");
            }

            factura.Imagenes = new DocImagenesFactura
            {
                LogoArca = _reportImageService.GetLogoArca(),
                LogoEmpresa = _reportImageService.GetLogoEmpresa(),
                SelloPagado = _reportImageService.GetSelloPagado()
            };

            var pdf = _pdfService.GenerarFactura(factura);
            return File(pdf, "application/pdf");
        }

        [AllowAnonymous]
        [HttpGet("Public/{id}")]
        public async Task<IActionResult> DescargarFacturaPublicaAsync(string id)
        {
            DtoComprobantesFacturasPublicas? dtoFactura = await comprobantesService.BuscarLinkPublicoPdfAsync(id);
            if (dtoFactura == null) return NotFound("Factura no encontrada");
            if (!dtoFactura.Activo) return NotFound("Enlace inactivo");

            /*
            if (!_pdfStorageService.Exists(dtoFactura.NombrePdf, dtoFactura.Anio.ToString()))
                return NotFound("Archivo no encontrado.");

            var fileBytes = _pdfStorageService.ReadFile(dtoFactura.NombrePdf, dtoFactura.Anio.ToString());
            return File(fileBytes, "application/pdf", dtoFactura.NombrePdf);
            */
            DtoComprobanteReq dtoComprobante = new DtoComprobanteReq
            {
                CodTalonario = dtoFactura.CodTalonario,
                NroComprobante = dtoFactura.NroComprobante
            };
            return await DescargarFacturaAsync(dtoComprobante);
        }

        #region Envio de facturas
        [HttpPost("ListadoFacturasEnviar")]
        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoFacturasEnviarAsync([FromBody] DtoOpcionesListados opcionesListado)
        {
            return await comprobantesService.ListadoFacturasEnviarAsync(opcionesListado);
        }

        [HttpPost("RegistrarEnvio")]
        public async Task<ActionResult<ApiResponse<int>>> RegistrarEnvioAsync([FromBody] DtoRegistrarEnvioListReq listSolicitud)
        {
            ApiResponse<int> apiResponse = new();
            Resultados resultado = await comprobantesService.RegistrarEnvioAsync(listSolicitud);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Message = "Envio registrado";
            return apiResponse;
        }

        [HttpPost("ListadoColaEnvioEstados")]
        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoColaEnvioEstadosAsync([FromBody] DtoOpcionesListados opcionesListado)
        {
            return await comprobantesService.ListadoColaEnvioEstadosAsync(opcionesListado);
        }

        #endregion

        [HttpPost("ListadoComprobantesPropuesta")]
        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoComprobantesPropuestaAsync([FromBody] DtoOpcionesListados opcionesListado)
        {
            return await comprobantesService.ListadoComprobantesPropuestaAsync(opcionesListado);
        }

    }
}
