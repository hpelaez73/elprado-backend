using Dapper;
using ElPrado.Core.Domains;
using ElPrado.Core.Enums;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ColaEnvioFacturaRepository : RepositoryBaseEntidad<ColaEnvioFactura>
    {
        private ConfiguracionListado cfgListColaEnvioEstado;

        public ColaEnvioFacturaRepository(DbContext dbContext) : base(dbContext)
        {
            cfgListColaEnvioEstado = new ConfiguracionListado()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "fechaDesde",
                        Etiqueta = "Fecha desde",
                        TipoDato = TipoDatoListado.Fecha,
                        PermiteFiltrar = true,
                        CampoSql = "C.FECHA_CREACION"

                    },
                    new CamposListado()
                    {
                        Campo = "fechaHasta",
                        Etiqueta = "Fecha hasta",
                        TipoDato = TipoDatoListado.Fecha,
                        PermiteFiltrar = true,
                        CampoSql = "C.FECHA_CREACION"
                    },
                    new CamposListado()
                    {
                        Campo = "verPendientes",
                        Etiqueta = "Ver pendientes",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "verEnviando",
                        Etiqueta = "Ver enviando",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "verEnviados",
                        Etiqueta = "Ver enviados",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "verErrores",
                        Etiqueta = "Ver errores",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true
                    },
                }
            };

        }

        public async Task AgregarAsync(int codCliente, int codTalonario, string nroComprobante, string medioEnvio)
        {
            string sql = @" EXECUTE PROCEDURE POST_COLA_ENVIO_FACTURAS(@codCliente, @codTalonario, @nroComprobante, @medioEnvio)";
            
            await _connection.ExecuteAsync(sql, new { codCliente, codTalonario, nroComprobante, medioEnvio }, _transaction);
        }

        public async Task<ApiResponseListado<IEnumerable<dynamic>>> ListadoColaEnvioEstadosAsync(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoColaEnvioEstado> funcionesListados = new(cfgListColaEnvioEstado, opcionesListado);

            DateOnly fechaDesde = default;
            DateOnly fechaHasta = default;
            string estados = string.Empty;
            if (opcionesListado.ListFiltros != null)
            {
                DtoCamposFiltroListado? campoFiltro = null;

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
                    fechaHasta = fechaHasta.AddDays(1); // Para incluir el día completo en el filtro hasta
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("verPendientes", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor)
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual
                    && campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                {
                    estados = $"'{EstadoEnvioDomain.Pendiente}'";
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("verEnviando", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor)
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual
                    && campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                {
                    estados = (string.IsNullOrEmpty(estados) ? $"'{EstadoEnvioDomain.Enviando}'" : $"{estados},'{EstadoEnvioDomain.Enviando}'");
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("verEnviados", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor)
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual
                    && campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                {
                    estados = (string.IsNullOrEmpty(estados) ? $"'{EstadoEnvioDomain.Enviado}'" : $"{estados},'{EstadoEnvioDomain.Enviado}'");
                }

                campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("verErrores", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor)
                    && campoFiltro.TipoComparacion == TipoComparacion.Igual
                    && campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                {
                    estados = (string.IsNullOrEmpty(estados) ? $"'{EstadoEnvioDomain.Error}'" : $"{estados},'{EstadoEnvioDomain.Error}'");
                }
            }

            string sqlWhere = "WHERE 1=1";
            if (fechaDesde != default)
            {
                sqlWhere += $" AND C.FECHA_CREACION >= '{fechaDesde:yyyy-MM-dd}'";
            }
            if (fechaHasta != default)
            {
                sqlWhere += $" AND C.FECHA_CREACION < '{fechaHasta:yyyy-MM-dd}'";
            }
            if (!string.IsNullOrEmpty(estados)) {
                sqlWhere += $" AND C.ESTADO IN ({estados})";
            }

            string sqlFrom = @$"FROM COLA_ENVIOS_FACTURAS C
                                INNER JOIN CLIENTES CL ON CL.COD_CLIENTE = C.COD_CLIENTE
                                INNER JOIN TALONARIOS T ON T.COD_TALONARIO = C.COD_TALONARIO";

            string sqlCant = $@"SELECT COUNT(*) {sqlFrom} {sqlWhere}";

            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            C.FECHA_CREACION, C.FECHA_ULTIMO_INTENTO, C.INTENTOS, C.ESTADO,
                            CL.NOMBRE AS CLIENTE, T.DESCRIPCION AS TALONARIO, C.NRO_COMPROBANTE, C.MEDIO_ENVIO, C.ERROR
                            {sqlFrom} 
                            {sqlWhere}
                            ORDER BY C.FECHA_CREACION DESC";

            return await funcionesListados.ApiResponseAsync(sql, _connection, _transaction);
        }

    }
}
