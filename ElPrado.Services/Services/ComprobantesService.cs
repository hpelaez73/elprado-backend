using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Documents;
using ElPrado.Dto.Dtos;
using ElPrado.Reports.Mappers;
using ElPrado.Services.Interfaces;

namespace ElPrado.Services.Services
{
    public class ComprobantesService : ServiceBase
    {
        public ComprobantesService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public async Task<DtoComprobantes?> VisualizarAsync(int codTalonario, string nroComprobante)
        {
            return await _uow.Comprobantes.VisualizarAsync(codTalonario, nroComprobante);
        }

        public async Task<IEnumerable<DtoComprobantesFacturasElectronicas>> FacturasAsync(int periodo)
        {
            return await _uow.Comprobantes.FacturasAsync(_userContext.GetCodCliente(), new DateTime(periodo, 1, 1), new DateTime(periodo, 12, 31));
        }

        public async Task<List<int>> PeriodosFacturacionAsync()
        {
            DtoComprobantesPeriodo? periodo = await _uow.Comprobantes.PeriodosFacturacionAsync(_userContext.GetCodCliente());
            if (periodo == null || periodo.MinFecha == DateOnly.MinValue)
            {
                return new();
            }
            else
            {
                List<int> listPeriodos = new();
                for (int i = periodo.MaxFecha.Year; i >= periodo.MinFecha.Year; i--)
                {
                    listPeriodos.Add(i);
                }
                return listPeriodos;
            }
        }


        #region Envio de facturas
        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoFacturasEnviarAsync(DtoOpcionesListados opcionesListado)
        {
            return await _uow.Comprobantes.ListadoFacturasEnviarAsync(opcionesListado);
        }

        public async Task<Resultados> RegistrarEnvioAsync(DtoRegistrarEnvioListReq listSolicitud)
        {
            Resultados resultado = new();
            if (listSolicitud.ListComprobantes == null || listSolicitud.ListComprobantes.Count == 0)
            {
                resultado.Agregar("No se han ingresado datos de envío");
                return resultado;
            }

            try
            {
                foreach (DtoRegistrarEnvioReq solicitud in listSolicitud.ListComprobantes)
                {
                    if (solicitud.CodTalonario == 0 || solicitud.CodCliente == 0 || string.IsNullOrEmpty(solicitud.NroComprobante) || string.IsNullOrEmpty(solicitud.Email))
                    {
                        continue;
                    }
                    await _uow.ColaEnvioFactura.AgregarAsync(solicitud.CodCliente, solicitud.CodTalonario, solicitud.NroComprobante, solicitud.Email);
                }
                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
            return resultado;
        }
        #endregion

        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoComprobantesPropuestaAsync(DtoOpcionesListados opcionesListado)
        {
            return await _uow.Comprobantes.ListadoComprobantesPropuestaAsync(opcionesListado);
        }

        public async Task<DtoComprobantesFacturasPublicas?> BuscarLinkPublicoPdfAsync(string id)
        {
            return await _uow.Comprobantes.BuscarLinkPublicoPdfAsync(id);
        }

        public async Task<DocFactura?> FacturaPdfAsync(int codTalonario, string nroComprobante)
        {
            DtoComprobantes? comprobante = await _uow.Comprobantes.VisualizarAsync(codTalonario, nroComprobante);
            if (comprobante == null)
            {
                return null;
            }

            try
            {
                await _uow.Comprobantes.RegistrarDescargaPdfAsync(codTalonario, nroComprobante, _userContext.GetCodCliente());
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

            return FacturasMapper.MapToDoc(comprobante);
        }

        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoColaEnvioEstadosAsync(DtoOpcionesListados opcionesListado)
        {
            return await _uow.ColaEnvioFactura.ListadoColaEnvioEstadosAsync(opcionesListado);
        }

        public async Task<List<DtoComprobantesSinCae>> ComprobantesSinCaeAsync(IAfipWsfeGateway _afipWsfeGateway)
        {
            List<DtoComprobantesSinCae> listComprobantes = await _uow.Comprobantes.ComprobantesSinCaeAsync();

            if (listComprobantes.Count == 0)
            {
                return listComprobantes;
            }

            var ultimoComprobanteCache = new Dictionary<string, string>();

            foreach (var comprobante in listComprobantes)
            {
                string cacheKey = $"{comprobante.CodTalonario}";

                if (!ultimoComprobanteCache.ContainsKey(cacheKey))
                {
                    var response = await _afipWsfeGateway.ConsultarUltimoComprobanteAsync(comprobante.CodTipoComprobante, comprobante.CodTalonario);

                    if (response.Success && response.Data != null)
                    {
                        ultimoComprobanteCache[cacheKey] = response.Data.NroComprobante;
                    }
                    else
                    {
                        ultimoComprobanteCache[cacheKey] = string.Empty;
                    }
                }

                comprobante.AfipUltimoComprobante = ultimoComprobanteCache[cacheKey];
            }

            return listComprobantes;
        }

        public async Task<Resultados> SolicitarCaeAsync(IAfipWsfeGateway afipWsfeGateway)
        {
            Resultados resultado = new();

            List<DtoComprobantesSinCae> listComprobantes = await ComprobantesSinCaeAsync(afipWsfeGateway);

            if (listComprobantes == null || listComprobantes.Count == 0)
            {
                resultado.Agregar("No se han ingresado comprobantes para solicitar CAE");
                return resultado;
            }

            foreach (var comprobante in listComprobantes)
            {
                ApiResponse<DtoAfipWsfe> response;

                if (string.Compare(comprobante.NroComprobante, comprobante.AfipUltimoComprobante, StringComparison.Ordinal) <= 0)
                {
                    response = await afipWsfeGateway.ActualizarCAEAsync(comprobante.CodTalonario, comprobante.NroComprobante);
                }
                else
                {
                    response = await afipWsfeGateway.SolicitarCAEAsync(comprobante.CodTalonario, comprobante.NroComprobante);
                }

                if (!response.Success)
                {
                    resultado.Agregar($"Comprobante {comprobante.NroComprobante}: {response.Message}");
                }
            }

            return resultado;
        }
    }
}
