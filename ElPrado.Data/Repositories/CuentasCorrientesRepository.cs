using Dapper;
using ElPrado.Core.Enums;
using ElPrado.Data.Comun;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class CuentasCorrientesRepository : RepositoryBase
    {
        private ConfiguracionListado configuracionListadoResumenCuotas;

        public CuentasCorrientesRepository(DbContext dbContext) : base(dbContext)
        {
            configuracionListadoResumenCuotas = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "tipo",
                        Etiqueta = "Tipo",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "codigo",
                        Etiqueta = "Código",
                        TipoDato = TipoDatoListado.Entero,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "incluirImputado",
                        Etiqueta = "Incluir imputado",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "incluirAnulado",
                        Etiqueta = "Incluir anulado",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "incluirRefinanciado",
                        Etiqueta = "IncluirRefinanciado",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    }
                }
            };
        }

        public List<DtoCuentasCorrientes> ConsultaDeudaMercadoPago(int codCliente, DateTime fechaDia)
        {
            string sql = "SELECT * FROM CONSULTA_DEUDA_MERCADOPAGO(@codCliente, @fechaDia)";
            return _connection.Query<DtoCuentasCorrientes>(sql, new { codCliente, fechaDia }, _transaction).AsList();
        }

        public List<DtoCuentasCorrientesResumen> ResumenCuentas(int codPropuesta, bool mostrarBaja, bool mostrarInactiva, DateTime? fechaInteres, DateTime? fechaHasta)
        {
            string sql = @" SELECT * FROM CONSULTA_RESUMEN_CUENTAS(@codPropuesta, @mostrarBaja, @mostrarInactiva, @fechaInteres, @fechaHasta) C
                            ORDER BY C.ES_INDEPENDIENTE, C.FECHA_INICIO DESC";
            return _connection.Query<DtoCuentasCorrientesResumen>(sql, new { codPropuesta, mostrarBaja, mostrarInactiva, fechaInteres, fechaHasta }, _transaction).AsList();
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoResumenCuotas(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoCuotasResumenList> funcionesListados = new(configuracionListadoResumenCuotas, opcionesListado);

            if (opcionesListado.ListFiltros == null || opcionesListado.ListFiltros.Count == 0)
            {
                return new ApiResponseListado<IEnumerable<dynamic>>();
            }

            bool incluirImputado = false;
            DtoCamposFiltroListado? campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("incluirImputado", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                incluirImputado = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
            }

            bool incluirAnulado = false;
            campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("incluirAnulado", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                incluirImputado = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
            }

            bool incluirRefinanciado = false;
            campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("incluirRefinanciado", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                incluirImputado = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
            }

            string tipoCuenta = string.Empty;
            campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("tipo", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                tipoCuenta = campoFiltro.Valor;
            }

            string codigo = string.Empty;
            campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("codigo", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                codigo = campoFiltro.Valor;
            }

            string sqlFrom = string.Empty;
            string sqlWhere = string.Empty;
            string sqlCampos = string.Empty;
            if (tipoCuenta.Equals("CR"))
            {
                sqlFrom = "FROM CUOTAS C";
                sqlWhere = $"WHERE C.COD_CREDITO = {codigo}";
                sqlCampos = @"C.COD_CREDITO AS COD_CUENTA, C.CUOTA, C.PAGO,
                              C.INTERES, C.FECHA_VENCIMIENTO AS FECHA_CUOTA, 0 AS INACTIVO, 'CR' AS TIPO,";
            }
            else if (tipoCuenta.Equals("CP"))
            {
                sqlFrom = "FROM CUOTAS_PERIODICAS C";
                sqlWhere = $"WHERE C.COD_CONFIGURACION = {codigo}";
                sqlCampos = @"C.COD_CONFIGURACION AS COD_CUENTA, C.CUOTA, C.PAGO,
                              0 AS INTERES, C.FECHA_CUOTA, C.INACTIVO, 'CP' AS TIPO,";
            }
            else
            {
                return new ApiResponseListado<IEnumerable<dynamic>>();
            }
            sqlWhere += (incluirImputado) ? "" : " AND C.FECHA_RENDICION IS NULL";
            sqlWhere += (incluirAnulado) ? "" : " AND C.ANULADO = 0";
            sqlWhere += (incluirRefinanciado) ? "" : " AND C.REFINANCIADO_POR IS NULL";

            string sqlCant = $@"SELECT COUNT(*)
                            {sqlFrom} 
                            {sqlWhere}";

            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            {sqlCampos}                            
                            C.TOTAL, C.FECHA_RENDICION, C.COD_TALONARIO_FACTURACION, C.NRO_COMPROBANTE_FACTURACION, C.COD_TALONARIO_IMPUTACION, C.NRO_COMPROBANTE_IMPUTACION,
                            C.MONTO, C.DESCUENTO, C.PUNITORIO, C.PUNITORIO - C.DESCUENTO AS DESCUENTO_RECARGO, C.ANULADO, C.COD_MOVIMIENTO_FONDO,
                            (SELECT U.NOMBRE FROM MOVIMIENTOS_FONDOS M INNER JOIN USUARIOS U ON U.COD_USUARIO = M.COD_USUARIO WHERE M.COD_MOVIMIENTO_FONDO = C.COD_MOVIMIENTO_FONDO) AS USUARIO,
                            IIF(EXISTS(SELECT 1 FROM EMISIONES_DEBITOS_AUTOMATICOS ED
                                INNER JOIN DET_EMISIONES_DEBITOS_COMP DD ON DD.COD_EMISION_DEBITO_AUTOMATICO = ED.COD_EMISION_DEBITO_AUTOMATICO
                                WHERE ED.FECHA_IMPUTACION IS NULL AND DD.RECHAZADO = 0 AND DD.COD_TALONARIO = C.COD_TALONARIO_EMISION AND DD.NRO_COMPROBANTE = C.NRO_COMPROBANTE_EMISION), 1, 0) AS EN_TARJETA,
                            IIF(EXISTS(SELECT 1 FROM EMISIONES_DEBITOS_CBU ED
                                INNER JOIN DET_EMISIONES_CBU_COMP DD ON DD.COD_EMISION_DEBITO_CBU = ED.COD_EMISION_DEBITO_CBU
                                WHERE ED.FECHA_IMPUTACION IS NULL AND DD.RECHAZADO = 0 AND DD.COD_TALONARIO = C.COD_TALONARIO_EMISION AND DD.NRO_COMPROBANTE = C.NRO_COMPROBANTE_EMISION), 1, 0) AS EN_CBU,
                            IIF(C.REFINANCIADO_POR IS NOT NULL, 1, 0) AS REFINANCIADO
                            {sqlFrom} 
                            {sqlWhere}
                            ORDER BY 1, 2 DESC, 3 DESC";

            return funcionesListados.ApiResponse(sql, _connection, _transaction);
        }

    }
}
