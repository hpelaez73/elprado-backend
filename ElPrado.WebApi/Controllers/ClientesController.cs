using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;

namespace ElPrado.WebApi.Controllers
{
    public class ClientesController : ControladorBaseCrud<Clientes, DtoClientes>
    {
        public ClientesController() { }

        protected override ServiceBaseCrud<Clientes, DtoClientes> CrearServicio()
        {
            return new ClientesService(null);
        }
    }
}
