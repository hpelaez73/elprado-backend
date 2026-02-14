using Dapper;
using ElPrado.Data.Comun;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class UsuariosRepository : RepositoryBaseCrud<Usuarios, DtoUsuarios>
    {
        private ConfiguracionListado cfgListUsuario;

        public UsuariosRepository(DbContext dbContext) : base(dbContext)
        {
            cfgListUsuario = new()
            {
                ListCampos = new()
                {
                    new CamposListado()
                    {
                        Campo = "alias",
                        Etiqueta = "Alias",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        PermiteOrdenar = true,
                        OrdenDefault = true,
                        CampoSql = "U.ALIAS"
                    },
                    new CamposListado()
                    {
                        Campo = "nombre",
                        Etiqueta = "Nombre",
                        TipoDato = TipoDatoListado.Texto,
                        PermiteFiltrar = true,
                        CampoSql = "U.NOMBRE"
                    }
                }
            };
        }

        public override DtoUsuarios? Visualizar(int id)
        {
            string sql = "SELECT * FROM USUARIOS WHERE COD_USUARIO = @id";
            return _connection.QuerySingleOrDefault<DtoUsuarios?>(sql, new { id }, _transaction);
        }

        public Usuarios? Buscar(string alias, string clave)
        {
            // Uso interpolación para soportar el uso de ñ y acentos en los alias y claves
            string sql = $"SELECT * FROM USUARIOS WHERE ALIAS = '{alias}' AND CLAVE_ACCESO = '{clave}'";
            List<Usuarios> listUsuarios = _connection.Query<Usuarios>(sql, null, _transaction).AsList();
            return listUsuarios.Count > 0 ? listUsuarios[0] : null;
        }

        public Usuarios? Buscar(int codUsuario, string clave)
        {
            string sql = $"SELECT * FROM USUARIOS WHERE COD_USUARIO = {codUsuario} AND CLAVE_ACCESO = '{clave}'";
            List<Usuarios> listUsuarios = _connection.Query<Usuarios>(sql, null, _transaction).AsList();
            return listUsuarios.Count > 0 ? listUsuarios[0] : null;
        }

        public List<MenusWeb> BuscarMenuUsuario(int codUsuario, bool soloPanel)
        {
            string sql = "SELECT * FROM MENU_WEB(@codUsuario, @soloPanel)";
            return _connection.Query<MenusWeb>(sql, new { codUsuario, soloPanel }, _transaction).AsList();
        }

        public List<MenusWeb> BuscarMenuCliente(bool soloPanel)
        {
            string sql = "SELECT * FROM MENU_WEB(NULL, @soloPanel)";
            return _connection.Query<MenusWeb>(sql, new { soloPanel }, _transaction).AsList();
        }

        public override ApiResponseListado<IEnumerable<dynamic>> Listado(DtoOpcionesListados opcionesListado)
        {
            FuncionesListados<DtoUsuarios> funcionesListados = new(cfgListUsuario, opcionesListado);

            string sqlWhere = (opcionesListado.ListFiltros == null || opcionesListado.ListFiltros.Count == 0) ? string.Empty : " WHERE " + funcionesListados.ParseSqlWhere();

            string sqlCant = $@"SELECT COUNT(*)
                            FROM USUARIOS U
                            {sqlWhere}";

            string sql = $@"SELECT {funcionesListados.ParseSqlPaginado(sqlCant, _connection, _transaction)}
                            U.COD_USUARIO, U.ALIAS, U.NOMBRE, U.CLAVE_ACCESO,
                            U.ES_ADMIN, U.SUPER_USUARIO, U.PUEDE_AUTORIZAR
                            FROM USUARIOS U
                            {sqlWhere}
                            {funcionesListados.ParseSqlOrden()}";

            return funcionesListados.ApiResponse(sql, _connection, _transaction);
        }
    }
}
