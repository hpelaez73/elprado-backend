using Dapper;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ObituariosRepository : RepositoryBaseCrud<Obituarios, DtoObituarios>
    {
        private ConfiguracionListado cfgListObituario;

        public ObituariosRepository(DbContext dbContext) : base(dbContext)
        {
            cfgListObituario = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "nombre",
                        Etiqueta = "Nombre",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        CampoSql = "O.NOMBRE"
                    }
                }
            };
        }

        public override DtoObituarios? Visualizar(int id)
        {
            string sql = "SELECT * FROM OBITUARIOS WHERE COD_OBITUARIO = @id";
            return _connection.QuerySingleOrDefault<DtoObituarios>(sql, new { id }, _transaction);
        }

        public DtoObituarioDetalleResp? BuscarDetalle(int id)
        {
            string sql = "SELECT * FROM OBITUARIOS WHERE COD_OBITUARIO = @id";
            return _connection.QuerySingleOrDefault<DtoObituarioDetalleResp>(sql, new { id }, _transaction);
        }

        public List<DtoReaccionesObituarios> BuscarReacciones(int id)
        {
            string sql = "SELECT * FROM REACCIONES_OBITUARIOS R WHERE R.COD_OBITUARIO = @id";
            return _connection.Query<DtoReaccionesObituarios>(sql, new { id }, _transaction).ToList();
        }

        public DtoDatabaseResp AgregarReaccion(int codObituario, string tipoReaccion)
        {
            string sql = "SELECT * FROM POST_AGREGAR_REACCION(@codObituario, @tipoReaccion)";
            return _connection.QuerySingle<DtoDatabaseResp>(sql, new { codObituario, tipoReaccion }, _transaction);
        }

        public void ActualizarImagen(int codObituario, string urlImagen)
        {
            string sql = @" UPDATE OBITUARIOS O SET O.URL_IMAGEN = @urlImagen
                            WHERE O.COD_OBITUARIO = @codObituario";
            _connection.Execute(sql, new { codObituario, urlImagen }, _transaction);
        }

        public override ApiResponseListado<IEnumerable<dynamic>> Listado(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoObituarios> funcionesListados = new(cfgListObituario, opcionesListado);

            string sqlWhere = " WHERE 1=1" + funcionesListados.ParseSqlWhere();

            string sqlCant = $@"SELECT COUNT(*)
                            FROM OBITUARIOS O
                            {sqlWhere}";

            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            O.*
                            FROM OBITUARIOS O
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            return funcionesListados.ApiResponse(sql, _connection, _transaction);
        }
    }
}
