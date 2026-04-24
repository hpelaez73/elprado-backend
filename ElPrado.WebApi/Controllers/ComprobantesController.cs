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
            if (!_pdfStorageService.Exists(fileName, year))
                return NotFound("Archivo no encontrado.");

            var fileBytes = _pdfStorageService.ReadFile(fileName, year);
            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpPost("Factura")]
        public IActionResult DescargarFactura([FromBody] DtoComprobanteReq dtoComprobante)
        {
            DocFactura? factura = comprobantesService.FacturaPdf(dtoComprobante.CodTalonario, dtoComprobante.NroComprobante);

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
        public IActionResult DescargarFactura(string id)
        {
            DtoComprobantesFacturasPublicas? dtoFactura = comprobantesService.BuscarLinkPublicoPdf(id);
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
            return DescargarFactura(dtoComprobante);
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
