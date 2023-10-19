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
            List<Usuarios> listUsuarios = conexion.GetList<Usuarios>(new { Alias = alias, ClaveAcceso = clave }, transaccion).AsList();
            return listUsuarios.Count > 0 ? listUsuarios[0] : null;
        }
    }
}
