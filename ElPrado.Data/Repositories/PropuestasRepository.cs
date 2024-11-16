using Dapper;
using ElPrado.Core.Enums;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class PropuestasRepository : RepositoryBaseEntidad<Propuestas>
    {
        private ConfiguracionListado configuracionListadoTitulares;
        private ConfiguracionListado configuracionListadoInhumados;
        private ConfiguracionListado configuracionListadoBeneficiarios;

        public PropuestasRepository(Transaccion transaccion) : base(transaccion)
        {
            configuracionListadoTitulares = new()
            {
                ListCampos = new() 
                {
                    new CamposListado()
                    {
                        Campo = "nombre",
                        Etiqueta = "Nombre",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = true,
                        OrdenDefault = true,
                        CampoSql = "CL.NOMBRE"
                    },
                    new CamposListado() {
                        Campo = "nroDocumento",
                        Etiqueta = "Nro de documento",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false,
                        CampoSql = "CL.NRO_DOCUMENTO"
                    },
                    new CamposListado() {
                        Campo = "incluirBaja",
                        Etiqueta = "Incluir baja",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false
                    }
                }
            };

            configuracionListadoInhumados = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "nombre",
                        Etiqueta = "Nombre",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = true,
                        OrdenDefault = true,
                        CampoSql = "COALESCE(CL.NOMBRE, I.NOMBRE_INHUMADO_NN)",
                        CampoSql2 = "I.NOMBRE_INHUMADO",
                        CampoSqlOrden = 1
                    },
                    new CamposListado() {
                        Campo = "nroDocumento",
                        Etiqueta = "Nro de documento",
                        TipoDato = TipoDatoListado.Texto,
                        CampoSql = "CL.NRO_DOCUMENTO",
                        CampoSql2 = "I.NUMERO_DOCUMENTO_INHUMADO",
                        PermiteFiltrar = true,
                        PermiteOrdenar = false
                    },
                    new CamposListado() {
                        Campo = "incluirBaja",
                        Etiqueta = "Incluir baja",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false
                    }
                }
            };

            configuracionListadoBeneficiarios = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "nombre",
                        Etiqueta = "Nombre",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = true,
                        OrdenDefault = true,
                        CampoSql = "CL.NOMBRE"
                    },
                    new CamposListado() {
                        Campo = "nroDocumento",
                        Etiqueta = "Nro de documento",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false,
                        CampoSql = "CL.NRO_DOCUMENTO"
                    },
                    new CamposListado() {
                        Campo = "incluirBaja",
                        Etiqueta = "Incluir baja",
                        TipoDato = TipoDatoListado.Boolean,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false
                    }
                }
            };
        }

        public Propuestas? BuscarPropuesta(int legajo)
        {
            List<Propuestas> listPropuestas = conexion.GetList<Propuestas>(new { Legajo = legajo }, transaccion).AsList();
            return listPropuestas.Count == 0 ? null : listPropuestas[0];
        }

        public Propuestas? BuscarPropuesta(string parcela, bool incluirBaja)
        {
            string sql = @" SELECT FIRST 1 P.*
                            FROM PROPUESTA P
                            INNER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA
                            WHERE (P.FECHA_BAJA IS NULL OR 1 = @incluirBaja)
                            AND PA.LEGAJO = @parcela
                            ORDER BY P.FECHA_BAJA DESC NULLS FIRST";
            return conexion.QuerySingleOrDefault<Propuestas>(sql, new { parcela, incluirBaja }, transaccion);
        }

        public DtoPropuestaDetalleResp? BuscarPropuestaDetalle(int codPropuesta)
        {
            string sql = @" SELECT * FROM GET_CONSULTA_PROPUESTA(@codPropuesta) P";
            return conexion.QuerySingleOrDefault<DtoPropuestaDetalleResp>(sql, new { codPropuesta }, transaccion);
        }

        public List<DtoPropuestasAsociadas> BuscarPropuestasAsociadas(int codPropuesta, bool incluirBajas)
        {
            string sql = "SELECT * FROM GET_DATOS_PROPUESTAS_ASOCIADAS(@codPropuesta, @incluirBajas)";
            return conexion.Query<DtoPropuestasAsociadas>(sql, new { codPropuesta, incluirBajas }, transaccion).ToList();
        }

        public List<DtoClientesPropuestasHistorial> DetalleHistorialTitulares(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_HISTORIAL_TITULARES(@codPropuesta)";
            return conexion.Query<DtoClientesPropuestasHistorial>(sql, new { codPropuesta }, transaccion).ToList();
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoInhumados(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoPropuestasInhumadosList> funcionesListados = new(configuracionListadoInhumados, opcionesListado);

            string sqlWhere1 = string.Empty;
            string sqlWhere2 = string.Empty;
            if (opcionesListado.ListFiltros != null)
            {
                bool incluirBaja = false;
                DtoCamposFiltroListado? campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("incluirBaja", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    incluirBaja = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
                }
                sqlWhere1 = (incluirBaja) ? " AND P.FECHA_BAJA IS NULL" : "";
                sqlWhere2 = (incluirBaja) ? " AND P.FECHA_BAJA IS NULL" : "";
                sqlWhere1 += funcionesListados.ParseSqlWhere();
                sqlWhere2 += funcionesListados.ParseSqlWhere(2);
            }

            string sqlFrom1 = @"FROM PROPUESTA P
                                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                                INNER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA
                                INNER JOIN DET_INHUMADOS DI ON DI.COD_PARCELA = P.COD_PARCELA AND DI.COD_PROPUESTA = P.COD_PROPUESTA
                                INNER JOIN INHUMADOS I ON I.COD_INHUMADO = DI.COD_INHUMADO
                                LEFT OUTER JOIN CLIENTES CL ON CL.COD_CLIENTE = I.COD_CLIENTE_INHUMADO
                                WHERE NOT EXISTS(SELECT 1 FROM INHUMACION IOLD WHERE IOLD.COD_INHUMADO_MIG = DI.COD_DET_INHUMADO AND IOLD.FECHA_CONTROL_MIG IS NULL)";
            string sqlFrom2 = @"FROM PROPUESTA P
                                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                                INNER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA
                                INNER JOIN INHUMACION I ON I.COD_PARCELA = P.COD_PARCELA AND I.COD_PROPUESTA = P.COD_PROPUESTA
                                WHERE I.FECHA_CONTROL_MIG IS NULL";
            string sqlCant = $@"SELECT 
                                  (SELECT COUNT(*) {sqlFrom1} {sqlWhere1})
                                + (SELECT COUNT(*) {sqlFrom2} {sqlWhere2}) AS CANT
                                FROM RDB$DATABASE";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, conexion, transaccion)}
                            I.NOMBRE_INHUMADO, I.FECHA_INHUMACION, I.NRO_DOCUMENTO,
                            I.COD_PROPUESTA, I.PROPUESTA, I.PARCELA,
                            I.FECHA, I.FECHA_BAJA
                            FROM (
                            SELECT
                            COALESCE(CL.NOMBRE, I.NOMBRE_INHUMADO_NN) AS NOMBRE_INHUMADO, DI.FECHA_INHUMACION, CL.NRO_DOCUMENTO,
                            P.COD_PROPUESTA, P.LEGAJO AS PROPUESTA, PA.LEGAJO AS PARCELA,
                            P.FECHA, P.FECHA_BAJA
                            {sqlFrom1} 
                            {sqlWhere1}
                            UNION ALL
                            SELECT I.NOMBRE_INHUMADO, CAST(I.FECHA_INHU_INHUMADO AS TIMESTAMP) AS FECHA_INHUMACION, I.NUMERO_DOCUMENTO_INHUMADO AS NRO_DOCUMENTO,
                            P.COD_PROPUESTA, P.LEGAJO AS PROPUESTA, PA.LEGAJO AS PARCELA,
                            P.FECHA, P.FECHA_BAJA
                            {sqlFrom2}
                            {sqlWhere2}
                            {funcionesListados.ParseSqlOrden()}
                            ) I";

            return funcionesListados.ApiResponse(sql, conexion, transaccion); ;
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoTitulares(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoPropuestasTitularesList> funcionesListados = new(configuracionListadoTitulares, opcionesListado);

            bool incluirBaja = false;
            if (opcionesListado.ListFiltros != null)
            {
                DtoCamposFiltroListado? campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("incluirBaja", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    incluirBaja = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
                }
            }
            string sqlWhere = ((incluirBaja) ? " WHERE P.FECHA_BAJA IS NULL" : " WHERE 1=1") + funcionesListados.ParseSqlWhere();
            string sqlFrom = @" FROM PROPUESTA P
                                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                                INNER JOIN PROPUESTAS_TITULARES CC ON CC.COD_PROPUESTA = P.COD_PROPUESTA AND CC.FECHA_BAJA IS NULL
                                INNER JOIN CLIENTES CL ON CL.COD_CLIENTE = CC.COD_CLIENTE
                                LEFT OUTER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA";
            string sqlCant = $@"SELECT COUNT(*)
                            {sqlFrom} 
                            {sqlWhere}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, conexion, transaccion)}
                            CL.NOMBRE, CL.NRO_DOCUMENTO, P.COD_PROPUESTA, P.LEGAJO AS PROPUESTA, PA.LEGAJO AS PARCELA,
                            P.FECHA, P.FECHA_BAJA, CL.COD_CLIENTE
                            {sqlFrom} 
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            return funcionesListados.ApiResponse(sql, conexion, transaccion);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoBeneficiarios(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoPropuestasTitularesList> funcionesListados = new(configuracionListadoBeneficiarios, opcionesListado);

            bool incluirBaja = false;
            if (opcionesListado.ListFiltros != null)
            {
                DtoCamposFiltroListado? campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("incluirBaja", StringComparison.CurrentCultureIgnoreCase));
                if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
                {
                    incluirBaja = campoFiltro.Valor.Equals("True", StringComparison.CurrentCultureIgnoreCase);
                }
            }
            string sqlWhere = "WHERE CL.COD_CLIENTE IS NOT NULL" + ((incluirBaja) ? " AND P.FECHA_BAJA IS NULL" : "") + funcionesListados.ParseSqlWhere();
            string sqlFrom = @" FROM HIST_SERVICIOS_UTILIZADOS H
                                INNER JOIN PROPUESTA P ON P.COD_PROPUESTA = H.COD_PROPUESTA
                                LEFT OUTER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA
                                LEFT OUTER JOIN INHUMADOS I ON I.COD_INHUMADO = H.COD_INHUMADO
                                LEFT OUTER JOIN CLIENTES CL ON CL.COD_CLIENTE = COALESCE(H.COD_CLIENTE_BENEFICIADO, I.COD_CLIENTE_INHUMADO)";
            string sqlCant = $@"SELECT COUNT(*)
                            {sqlFrom} 
                            {sqlWhere}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, conexion, transaccion)}
                            DISTINCT CL.NOMBRE, CL.NRO_DOCUMENTO, P.LEGAJO AS PROPUESTA, PA.LEGAJO AS PARCELA, H.COD_PROPUESTA,
                            P.FECHA, P.FECHA_BAJA
                            {sqlFrom} 
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            return funcionesListados.ApiResponse(sql, conexion, transaccion);
        }

    }
}
