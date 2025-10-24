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
                        PermiteFiltrar = true,
                        PermiteOrdenar = true,
                        OrdenDefault = true,
                        CampoSql = "P.LEGAJO"
                    },
                    new CamposListado()
                    {
                        Campo = "fecha",
                        Etiqueta = "Fecha",
                        TipoDato = TipoDatoListado.Fecha,
                        PermiteFiltrar = true,
                        PermiteOrdenar = true,
                        OrdenDefault = true,
                        CampoSql = "C.FECHA"
                    },
                    new CamposListado()
                    {
                        Campo = "soloNuevo",
                        Etiqueta = "Solo nuevo",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false
                    },
                    new CamposListado()
                    {
                        Campo = "posteriorSolicitud",
                        Etiqueta = "Posterior a la solicitud",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false
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

        public DtoComprobantes? Visualizar(int codTalonario, string nroComprobante)
        {
            string sql = "SELECT * FROM GET_DATOS_COMPROBANTE(@codTalonario, @nroComprobante)";
            DtoComprobantes? comprobante = _connection.QuerySingleOrDefault<DtoComprobantes>(sql, new { codTalonario, nroComprobante }, _transaction);

            if (comprobante != null)
            {
                sql = "SELECT * FROM GET_DATOS_COMPROBANTE_DETALLE(@codTalonario, @nroComprobante)";
                comprobante.ListDetalles = _connection.Query<DtoComprobantesDetalles>(sql, new { codTalonario, nroComprobante }, _transaction).ToList();
            }

            return comprobante;
        }

        public IEnumerable<DtoComprobantesFacturasElectronicas> Facturas(int codCliente, DateTime fechaDesde, DateTime fechaHasta, string basePath)
        {
            string sql = "SELECT * FROM GET_FACTURAS_AFIP(@codCliente, @fechaDesde, @fechaHasta, @basePath)";
            return _connection.Query<DtoComprobantesFacturasElectronicas>(sql, new { codCliente , fechaDesde, fechaHasta, basePath }, _transaction);
        }

        public DtoComprobantesPeriodo? PeriodosFacturacion(int codCliente)
        {
            string sql = "SELECT * FROM GET_PERIODOS_FACTURAS_AFIP(@codCliente)";
            return _connection.QuerySingleOrDefault<DtoComprobantesPeriodo?>(sql, new { codCliente }, _transaction);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoFacturasEnviar(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoFacturasEnviarList> funcionesListados = new(cfgListFacturasEnviar, opcionesListado);

            string sqlWhere = $@"WHERE C.AFIP_CAE IS NOT NULL AND S.FECHA_CANCELACION IS NULL
                                AND S.POR_WHATSAPP = 1 AND CL.TELEFONO_MOVIL IS NOT NULL
                                AND T.ES_FACTURA_ELECTRONICA = 1 AND TC.TIPO_COMPROBANTE = '{TipoComprobanteDomain.Factura}'";
            if (opcionesListado.ListFiltros != null)
            {
                DtoCamposFiltroListado? campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("soloNuevo", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    sqlWhere += @"  AND NOT EXISTS (SELECT 1 FROM HIST_ENVIOS_FACTURAS H
                                    WHERE H.COD_CLIENTE = S.COD_CLIENTE AND H.COD_TALONARIO = C.COD_TALONARIO AND H.NRO_COMPROBANTE = C.NRO_COMPROBANTE)";
                }
                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("posteriorSolicitud", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    sqlWhere += @"  AND C.FECHA >= S.FECHA_PEDIDO";
                }
            }
            sqlWhere += funcionesListados.ParseSqlWhere();

            string sqlFrom = @" FROM SOLICITUDES_ENVIOS_FACTURAS S
                                INNER JOIN CLIENTES CL ON CL.COD_CLIENTE = S.COD_CLIENTE
                                INNER JOIN COMPROBANTES C ON C.COD_PROPUESTA = S.COD_PROPUESTA AND C.COD_TALONARIO BETWEEN 101 AND 106
                                INNER JOIN PROPUESTA P ON P.COD_PROPUESTA = S.COD_PROPUESTA
                                INNER JOIN TALONARIOS T ON T.COD_TALONARIO = C.COD_TALONARIO
                                INNER JOIN TIPOS_COMPROBANTES TC ON TC.COD_TIPO_COMPROBANTE = C.COD_TIPO_COMPROBANTE";
            string sqlCant = $@"SELECT COUNT(*)
                            {sqlFrom} 
                            {sqlWhere}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            P.LEGAJO AS PROPUESTA, C.NRO_COMPROBANTE, C.COD_TALONARIO, P.COD_PROPUESTA, 
                            CL.TELEFONO_MOVIL, CL.NOMBRE AS CLIENTE, C.FECHA, CL.COD_CLIENTE,
                            (SELECT MAX(H.FECHA_ENVIO) FROM HIST_ENVIOS_FACTURAS H
                            WHERE H.COD_CLIENTE = S.COD_CLIENTE AND H.COD_TALONARIO = C.COD_TALONARIO AND H.NRO_COMPROBANTE = C.NRO_COMPROBANTE) AS ULTIMO_ENVIO
                            {sqlFrom} 
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            return funcionesListados.ApiResponse(sql, _connection, _transaction);
        }

        public void RegistrarEnvio(int codCliente, int codTalonario, string nroComprobante, string medioEnvio, int codUsuario)
        {
            string sql = @" INSERT INTO HIST_ENVIOS_FACTURAS (FECHA_ENVIO, POR_WHATSAPP, COD_USUARIO, COD_CLIENTE, COD_TALONARIO, NRO_COMPROBANTE, MEDIO_ENVIO)
                            VALUES (CURRENT_TIMESTAMP, 1, @codUsuario, @codCliente, @codTalonario, @nroComprobante, @medioEnvio)";
            _connection.Execute(sql, new { codUsuario, codCliente, codTalonario, nroComprobante, medioEnvio }, _transaction);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoComprobantesPropuesta(DtoOpcionesListados opcionesListado)
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

            return funcionesListados.ApiResponse(sql, _connection, _transaction);
        }

    }
}
