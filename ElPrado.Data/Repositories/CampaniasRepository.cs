using ElPrado.Core.Enums;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class CampaniasRepository : RepositoryBaseEntidad<Campanias>
    {
        private ConfiguracionListado cfgListCampaniasPropuesta;

        public CampaniasRepository(DbContext dbContext) : base(dbContext)
        {
            cfgListCampaniasPropuesta = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "codPropuesta",
                        Etiqueta = "Propuesta",
                        TipoDato = TipoDatoListado.Entero,
                        PermiteFiltrar = true
                    },
                    new CamposListado()
                    {
                        Campo = "codUsuario",
                        Etiqueta = "Usuario",
                        TipoDato = TipoDatoListado.Entero,
                        PermiteFiltrar = true
                    }
                }
            };
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoCampaniasPropuesta(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoCampaniasPropuestaList> funcionesListados = new(cfgListCampaniasPropuesta, opcionesListado);

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
            string codUsuario = string.Empty;
            campoFiltro = opcionesListado.ListFiltros.Find(x => x.Campo.Equals("codUsuario", StringComparison.CurrentCultureIgnoreCase));
            if (campoFiltro != null && !string.IsNullOrEmpty(campoFiltro.Valor) && campoFiltro.TipoComparacion == TipoComparacion.Igual)
            {
                codUsuario = campoFiltro.Valor;
            }

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(codUsuario))
            {
                return new ApiResponseListado<IEnumerable<dynamic>>();
            }

            string sqlFrom = $"FROM GET_CAMPANIAS_PROPUESTA({codigo}, {codUsuario}) CP";
            string sqlCant = $"SELECT COUNT(*) {sqlFrom}";
            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            CP.* 
                            {sqlFrom}
                            {funcionesListados.ParseSqlOrden()}";

            return funcionesListados.ApiResponse(sql, _connection, _transaction);

        }
    }
}
