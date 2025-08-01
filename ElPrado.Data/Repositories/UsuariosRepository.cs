using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class UsuariosRepository : RepositoryBaseCrud<Usuarios, DtoUsuarios>
    {
        public UsuariosRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public Usuarios? Buscar(string alias, string clave)
        {
            // Uso interpolación para soportar el uso de ñ y acentos en los alias y claves
            string sql = $"SELECT * FROM USUARIOS WHERE ALIAS = '{alias}' AND CLAVE_ACCESO = '{clave}'";
            List<Usuarios> listUsuarios = connection.Query<Usuarios>(sql, null, transaction).AsList();
            return listUsuarios.Count > 0 ? listUsuarios[0] : null;
        }

        public Usuarios? Buscar(int codUsuario, string clave)
        {
            string sql = $"SELECT * FROM USUARIOS WHERE COD_USUARIO = {codUsuario} AND CLAVE_ACCESO = '{clave}'";
            List<Usuarios> listUsuarios = connection.Query<Usuarios>(sql, null, transaction).AsList();
            return listUsuarios.Count > 0 ? listUsuarios[0] : null;
        }

        public List<MenusWeb> BuscarMenuUsuario(int codUsuario, bool soloPanel)
        {
            string sql = "SELECT * FROM MENU_WEB(@codUsuario, @soloPanel)";
            return connection.Query<MenusWeb>(sql, new { codUsuario, soloPanel }, transaction).AsList();
        }

        public List<MenusWeb> BuscarMenuCliente(bool soloPanel)
        {
            string sql = "SELECT * FROM MENU_WEB(NULL, @soloPanel)";
            return connection.Query<MenusWeb>(sql, new { soloPanel }, transaction).AsList();
        }

    }
}
