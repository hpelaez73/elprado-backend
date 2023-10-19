using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;

namespace ElPrado.WebApi.Controllers
{
    public class UsuariosController : ControladorBaseCrud<Usuarios, DtoUsuarios>
    {
        public UsuariosController()
        {
        }

        protected override ServiceBaseCrud<Usuarios, DtoUsuarios> CrearServicio()
        {
            return new UsuariosService(null);
        }
    }
}
