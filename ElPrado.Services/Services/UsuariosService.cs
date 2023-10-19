using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class UsuariosService : ServiceBaseCrud<Usuarios, DtoUsuarios>
    {
        private UsuariosRepository usuariosRepository => (repository as UsuariosRepository)!;

        public UsuariosService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBaseCrud<Usuarios, DtoUsuarios> CrearRepositorio()
        {
            return new UsuariosRepository(transaccion);
        }
    }
}
