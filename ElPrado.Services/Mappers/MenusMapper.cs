using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public static class MenusMapper
    {
        public static DtoMenus MapToDto(MenusWeb menusWeb, bool esUsuario)
        {
            return new DtoMenus()
            {
                EsUsuario = esUsuario,
                EsDivision = menusWeb.EsDivision,
                Etiqueta = menusWeb.Etiqueta,
                Icono = menusWeb.PathIcono,
                Link = menusWeb.Proceso,
                Items = MenusItemsMapper.MapToDto(menusWeb.Items, esUsuario)
            };
        }

        public static List<DtoMenus> MapToDto(List<MenusWeb> listMenusWeb, bool esUsuario)
        {
            return listMenusWeb.Select(item => MapToDto(item, esUsuario)).AsList();
        }
    }

    public static class MenusItemsMapper
    {
        public static DtoMenusItems MapToDto(MenusWeb menusWeb, bool esUsuario)
        {
            return new DtoMenusItems()
            {
                EsUsuario = esUsuario,
                EsDivision = menusWeb.EsDivision,
                Etiqueta = menusWeb.Etiqueta,
                Icono = menusWeb.PathIcono,
                Link = menusWeb.Proceso
            };
        }

        public static List<DtoMenusItems>? MapToDto(List<MenusWeb>? listMenusWeb, bool esUsuario)
        {
            if (listMenusWeb == null) return null;
            return listMenusWeb.Select(item => MapToDto(item, esUsuario)).AsList();
        }
    }

    public static class PanelMapper
    {
        public static DtoPanel MapToDto(MenusWeb menusWeb, bool esUsuario)
        {
            return new DtoPanel()
            {
                EsUsuario = esUsuario,
                Descripcion = menusWeb.Descripcion,
                Etiqueta = menusWeb.Etiqueta,
                Icono = menusWeb.PathIcono,
                Link = menusWeb.Proceso
            };
        }

        public static List<DtoPanel> MapToDto(List<MenusWeb> listMenusWeb, bool esUsuario)
        {
            return listMenusWeb.Select(item => MapToDto(item, esUsuario)).AsList();
        }
    }
}
