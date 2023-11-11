using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;
using System.Runtime.Serialization.Formatters;

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

        public List<DtoMenus> BuscarMenu()
        {
            List<MenusWeb> listMenusWeb = (ConfiguracionGeneralSesion.CodUsuario > 0)
                ? usuariosRepository.BuscarMenuUsuario(ConfiguracionGeneralSesion.CodUsuario, false)
                : usuariosRepository.BuscarMenuCliente(false);

            int maxNivel = listMenusWeb.Max(x => x.Nivel);
            for (int i = maxNivel; i > 1; i--)
            {
                foreach (MenusWeb item in listMenusWeb.Where(x => x.Nivel == i))
                {
                    MenusWeb? padre = listMenusWeb.Find(x => x.CodItemMenu == item.CodPadre);
                    if (padre != null)
                    {
                        padre.Items ??= new();
                        padre.Items.Add(item);
                    }
                }
                listMenusWeb.RemoveAll(x => x.Nivel == i);
            }
            return MenusMapper.MapToDto(listMenusWeb);
        }

        public List<DtoPanel> BuscarPanel()
        {
            List<MenusWeb> listMenusWeb = (ConfiguracionGeneralSesion.CodUsuario > 0)
                ? usuariosRepository.BuscarMenuUsuario(ConfiguracionGeneralSesion.CodUsuario, true)
                : usuariosRepository.BuscarMenuCliente(true);

            return PanelMapper.MapToDto(listMenusWeb);
        }
    }
}
