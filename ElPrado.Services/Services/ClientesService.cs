using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ClientesService : ServiceBaseCrud<Clientes, DtoClientes>
    {
        private ClientesRepository clientesRepository => (repository as ClientesRepository)!;

        public ClientesService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBaseCrud<Clientes, DtoClientes> CrearRepositorio()
        {
            return new ClientesRepository(transaccion);
        }
    }
}
