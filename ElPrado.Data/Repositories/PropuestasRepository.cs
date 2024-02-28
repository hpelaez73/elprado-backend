using Dapper;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class PropuestasRepository : RepositoryBaseEntidad<Propuestas>
    {
        private ConfiguracionListado configuracionListadoTitulares;
        private ConfiguracionListado configuracionListadoInhumados;

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
                        CampoSql = "CL.NOMBRE"
                    },
                    new CamposListado() {
                        Campo = "nroDocumento",
                        Etiqueta = "Nro de documento",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = false,
                        CampoSql = "CL.NRO_DOCUMENTO"
                    }
                }
            };
        }

        public Propuestas? BuscarPropuesta(int legajo)
        {
            List<Propuestas> listPropuestas = conexion.GetList<Propuestas>(new { Legajo = legajo }, transaccion).AsList();
            return listPropuestas.Count == 0 ? null : listPropuestas[0];
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoInhumados(DtoOpcionesListados opcionesListado)
        {
            ApiResponseListado<IEnumerable<dynamic>> apiResponse = new();

            FuncionesListados funcionesListados = new(configuracionListadoInhumados, opcionesListado);
            if (funcionesListados.MostrarFiltros())
            {
                apiResponse.ListFiltros = funcionesListados.MapOpcionesListado();
                return apiResponse;
            }

            string sqlWhere = "WHERE P.FECHA_BAJA IS NULL AND ED.ACTIVA = 1" + funcionesListados.ParseSqlWhere(false);
            string sqlFrom = @" FROM PROPUESTA P
                                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                                INNER JOIN PROPUESTAS_TITULARES CC ON CC.COD_PROPUESTA = P.COD_PROPUESTA AND CC.FECHA_BAJA IS NULL
                                INNER JOIN CLIENTES CL ON CL.COD_CLIENTE = CC.COD_CLIENTE
                                LEFT OUTER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA";
            string sqlCant = $@"SELECT COUNT(*)
                            {sqlFrom} 
                            {sqlWhere}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, conexion, transaccion)}
                            CL.NOMBRE, CL.NRO_DOCUMENTO, P.COD_PROPUESTA, P.LEGAJO AS PROPUESTA, PA.LEGAJO AS PARCELA
                            {sqlFrom} 
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            apiResponse.CantidadPaginas = funcionesListados.CantidadPaginas();
            apiResponse.Data = conexion.Query<DtoPropuestas>(sql, null, transaccion);
            return apiResponse;
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoTitulares(DtoOpcionesListados? opcionesListado)
        {
            ApiResponseListado<IEnumerable<dynamic>> apiResponse = new();

            FuncionesListados funcionesListados = new(configuracionListadoTitulares, opcionesListado);
            if (funcionesListados.MostrarFiltros())
            {
                apiResponse.ListFiltros = funcionesListados.MapOpcionesListado();
                return apiResponse;
            }

            string sqlWhere = "WHERE P.FECHA_BAJA IS NULL AND ED.ACTIVA = 1" + funcionesListados.ParseSqlWhere(false);
            string sqlFrom = @" FROM PROPUESTA P
                                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                                INNER JOIN PROPUESTAS_TITULARES CC ON CC.COD_PROPUESTA = P.COD_PROPUESTA AND CC.FECHA_BAJA IS NULL
                                INNER JOIN CLIENTES CL ON CL.COD_CLIENTE = CC.COD_CLIENTE
                                LEFT OUTER JOIN PARCELA PA ON PA.COD_PARCELA = P.COD_PARCELA";
            string sqlCant = $@"SELECT COUNT(*)
                            {sqlFrom} 
                            {sqlWhere}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, conexion, transaccion)}
                            CL.NOMBRE, CL.NRO_DOCUMENTO, P.COD_PROPUESTA, P.LEGAJO AS PROPUESTA, PA.LEGAJO AS PARCELA
                            {sqlFrom} 
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            apiResponse.CantidadPaginas = funcionesListados.CantidadPaginas();
            apiResponse.Data = conexion.Query<DtoPropuestas>(sql, null, transaccion);
            return apiResponse;
        }
    }
}
