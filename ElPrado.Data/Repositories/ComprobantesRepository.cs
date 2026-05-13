using Dapper;
using ElPrado.Core.Domains;
using ElPrado.Core.Enums;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ComprobantesRepository : RepositoryBaseEntidad<Comprobantes>
    {
        private ConfiguracionListado cfgListFacturasEnviar;
        private ConfiguracionListado cfgListComprobantesPropuesta;

        public ComprobantesRepository(DbContext dbContext) : base(dbContext)
        {
            cfgListFacturasEnviar = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "propuesta",
                        Etiqueta = "Propuesta",
                        TipoDato = TipoDatoListado.Entero,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "fechaDesde",
                        Etiqueta = "Fecha desde",
                        TipoDato = TipoDatoListado.Fecha,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "fechaHasta",
                        Etiqueta = "Fecha hasta",
                        TipoDato = TipoDatoListado.Fecha,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "soloNuevo",
                        Etiqueta = "Solo nuevo",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "posteriorSolicitud",
                        Etiqueta = "Posterior a la solicitud",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    }
                }
            };

            cfgListComprobantesPropuesta = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "codPropuesta",
                        Etiqueta = "Propuesta",
                        TipoDato = TipoDatoListado.Entero,
                        PermiteFiltrar = true
                    }
                }
            };
        }

        public async Task<DtoComprobantes?> VisualizarAsync(int codTalonario, string nroComprobante)
        {
            string sql = "SELECT * FROM GET_DATOS_COMPROBANTE(@codTalonario, @nroComprobante)";
            DtoComprobantes? comprobante = await _connection.QuerySingleOrDefaultAsync<DtoComprobantes>(sql, new { codTalonario, nroComprobante }, _transaction);

            if (comprobante != null)
            {
                sql = "SELECT * FROM GET_DATOS_COMPROBANTE_DETALLE(@codTalonario, @nroComprobante)";
                var detalles = await _connection.QueryAsync<DtoComprobantesDetalles>(sql, new { codTalonario, nroComprobante }, _transaction);
                comprobante.ListDetalles = detalles.ToList();
            }

            return comprobante;
        }

        public async Task<IEnumerable<DtoComprobantesFacturasElectronicas>> FacturasAsync(int codCliente, DateTime fechaDesde, DateTime fechaHasta)
        {
            string sql = "SELECT * FROM GET_FACTURAS_AFIP(@codCliente, @fechaDesde, @fechaHasta)";
            return await _connection.QueryAsync<DtoComprobantesFacturasElectronicas>(sql, new { codCliente , fechaDesde, fechaHasta }, _transaction);
        }

        public async Task<DtoComprobantesPeriodo?> PeriodosFacturacionAsync(int codCliente)
        {
            string sql = "SELECT * FROM GET_PERIODOS_FACTURAS_AFIP(@codCliente)";
            return await _connection.QuerySingleOrDefaultAsync<DtoComprobantesPeriodo?>(sql, new { codCliente }, _transaction);
        }

        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoFacturasEnviarAsync(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoFacturasEnviarList> funcionesListados = new(cfgListFacturasEnviar, opcionesListado);

            int codPropuesta = 0;
            bool soloNuevo = false;
            bool posteriorSolicitud = false;
            DateOnly fechaDesde = default;
            DateOnly fechaHasta = default;
            if (opcionesListado.ListFiltros != null)
            {
                DtoCamposFiltroListado? campoFiltro = null;

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("propuesta", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor)
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    _ = int.TryParse(campoFiltro.Valor, out codPropuesta);
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("soloNuevo", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) 
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    soloNuevo = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("posteriorSolicitud", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) 
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    posteriorSolicitud = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("fechaDesde", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) 
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    _ = DateOnly.TryParse(campoFiltro.Valor, out fechaDesde);
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("fechaHasta", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) 
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    _ = DateOnly.TryParse(campoFiltro.Valor, out fechaHasta);
                }
            }

            string sqlFrom = @$"FROM CONSULTA_FACTURAS_ENVIAR_MAIL({codPropuesta},
                                {(fechaDesde == default ? "NULL" : $"'{fechaDesde:yyyy-MM-dd}'")},
                                {(fechaHasta == default ? "NULL" : $"'{fechaHasta:yyyy-MM-dd}'")}, 
                                {(soloNuevo ? 1 : 0)}, {(posteriorSolicitud ? 1 : 0)}) C";

            string sqlCant = $@"SELECT COUNT(*) {sqlFrom}";
            
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)} C.*
                            {sqlFrom} 
                            ORDER BY C.FECHA, C.NRO_COMPROBANTE";

            return await funcionesListados.ApiResponseAsync(sql, _connection, _transaction);
        }

        public async Task RegistrarEnvioAsync(int codCliente, int codTalonario, string nroComprobante, string medioEnvio, int codUsuario)
        {
            string sql = @" INSERT INTO HIST_ENVIOS_FACTURAS (FECHA_ENVIO, POR_WHATSAPP, COD_USUARIO, COD_CLIENTE, COD_TALONARIO, NRO_COMPROBANTE, MEDIO_ENVIO)
                            VALUES (CURRENT_TIMESTAMP, 1, @codUsuario, @codCliente, @codTalonario, @nroComprobante, @medioEnvio)";
            await _connection.ExecuteAsync(sql, new { codUsuario, codCliente, codTalonario, nroComprobante, medioEnvio }, _transaction);
        }

        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoComprobantesPropuestaAsync(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoComprobantesPropuestaList> funcionesListados = new(cfgListComprobantesPropuesta, opcionesListado);

            if (opcionesListado.ListFiltros == null || opcionesListado.ListFiltros.Count == 0)
            {
                return new ApiResponseListado<IEnumerable<dynamic>>();
            }

            string codigo = string.Empty;
            DtoCamposFiltroListado? campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("codPropuesta", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                codigo = campoFiltro.Valor;
            }

            if (string.IsNullOrEmpty(codigo))
            {
                return new ApiResponseListado<IEnumerable<dynamic>>();
            }

            string sqlFrom = $"FROM GET_COMPROBANTES_PROPUESTA({ codigo }) CP";
            string sqlCant = $"SELECT COUNT(*) {sqlFrom}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            CP.* 
                            {sqlFrom}
                            {funcionesListados.ParseSqlOrden()}";

            return await funcionesListados.ApiResponseAsync(sql, _connection, _transaction);
        }

        public async Task<DtoComprobantesFacturasPublicas?> BuscarLinkPublicoPdfAsync(string id)
        {
            string sql = "SELECT * FROM GET_LINK_PUBLICOS_PDFS(@id)";
            return await _connection.QuerySingleOrDefaultAsync<DtoComprobantesFacturasPublicas?>(sql, new { id }, _transaction);
        }

        public async Task RegistrarDescargaPdfAsync(int codTalonario, string nroComprobante, int codCliente)
        {
            string sql = @" EXECUTE PROCEDURE POST_REGISTRO_DESCARGA_FACTURA(@codCliente, @codTalonario, @nroComprobante)";
            await _connection.ExecuteAsync(sql, new { codCliente, codTalonario, nroComprobante }, _transaction);
        }
    }
}
