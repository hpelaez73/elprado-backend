using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public static class MenusMapper
    {
        public static DtoMenus MapToDto(MenusWeb menusWeb)
        {
            return new DtoMenus()
            {
                EsDivision = menusWeb.EsDivision,
                Etiqueta = menusWeb.Etiqueta,
                Icono = menusWeb.PathIcono,
                Link = menusWeb.Proceso,
                Items = MenusItemsMapper.MapToDto(menusWeb.Items)
            };
        }

        public static List<DtoMenus> MapToDto(List<MenusWeb> listMenusWeb)
        {
            return listMenusWeb.Select(item => MapToDto(item)).AsList();
        }
    }

    public static class MenusItemsMapper
    {
        public static DtoMenusItems MapToDto(MenusWeb menusWeb)
        {
            return new DtoMenusItems()
            {
                EsDivision = menusWeb.EsDivision,
                Etiqueta = menusWeb.Etiqueta,
                Icono = menusWeb.PathIcono,
                Link = menusWeb.Proceso
            };
        }

        public static List<DtoMenusItems>? MapToDto(List<MenusWeb>? listMenusWeb)
        {
            if (listMenusWeb == null) return null;
            return listMenusWeb.Select(item => MapToDto(item)).AsList();
        }
    }

    public static class PanelMapper
    {
        public static DtoPanel MapToDto(MenusWeb menusWeb)
        {
            return new DtoPanel()
            {
                Descripcion = menusWeb.Descripcion,
                Etiqueta = menusWeb.Etiqueta,
                Icono = menusWeb.PathIcono,
                Link = menusWeb.Proceso
            };
        }

        public static List<DtoPanel> MapToDto(List<MenusWeb> listMenusWeb)
        {
            return listMenusWeb.Select(item => MapToDto(item)).AsList();
        }
    }
}
