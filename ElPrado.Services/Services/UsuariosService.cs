using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

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
            return new UsuariosRepository(Transaccion);
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

        public Resultados<DtoAutorizacionesSolicitudResp> AnalizarSolicitudAutorizacion(DtoAutorizacionesSolicitudReq solicitud)
        {
            Resultados<DtoAutorizacionesSolicitudResp> resultado = new();
            if (solicitud.Parte1.Trim() == string.Empty 
                || solicitud.Parte2.Trim() == string.Empty
                || solicitud.Parte3.Trim() == string.Empty) resultado.Agregar("La solicitud está incompleta");

            if (resultado.HayError) return resultado;

            VariosSeguridadRepository variosSeguridadRepository = new(Transaccion);
            DtoAutorizacionesSolicitudResp solicitudResp = new()
            {
                Proceso = variosSeguridadRepository.TraducirClave(solicitud.Parte1.Trim(), false),
                Usuario = variosSeguridadRepository.TraducirClave(solicitud.Parte2.Trim(), true)
            };

            if (solicitudResp.Usuario == string.Empty || solicitudResp.Proceso == string.Empty)
            {
                resultado.Agregar("El usuario o el proceso no existe");
                return resultado;
            }

            resultado.Valor = solicitudResp;
            return resultado;
        }

        public Resultados<DtoAutorizacionesGeneracionResp> GenerarAutorizacion(DtoAutorizacionesGeneracionReq solicitud)
        {
            Resultados<DtoAutorizacionesGeneracionResp> resultado = new();
            if (solicitud.Parte1.Trim() == string.Empty
                || solicitud.Parte2.Trim() == string.Empty
                || solicitud.Parte3.Trim() == string.Empty) resultado.Agregar("La solicitud está incompleta");

            if (solicitud.Clave.Trim() == string.Empty) resultado.Agregar("Falta ingresar la clave");

            DtoAutorizacionesSolicitudReq solicitudReq = new()
            {
                Parte1 = solicitud.Parte1,
                Parte2 = solicitud.Parte2,
                Parte3 = solicitud.Parte3
            };
            Resultados<DtoAutorizacionesSolicitudResp> resultadoInfo = AnalizarSolicitudAutorizacion(solicitudReq);
            if (resultadoInfo.HayError) resultado.Agregar(resultadoInfo);

            Usuarios? usuario = usuariosRepository.Buscar(ConfiguracionGeneralSesion.CodUsuario, solicitud.Clave);
            if (usuario == null)
            {
                resultado.Agregar("La clave es incorrecta");
                return resultado;
            }

            if (!usuario.PuedeAutorizar) resultado.Agregar("El usuario no esta autorizado a generar claves de autorización");

            if (resultado.HayError) return resultado;

            resultado.Valor = ArmarAutorizacion(solicitud);

            return resultado;
        }

        private DtoAutorizacionesGeneracionResp ArmarAutorizacion(DtoAutorizacionesGeneracionReq solicitud)
        {
            string str1 = Ajustar(solicitud.Parte1.Trim());
            string str2 = Ajustar(solicitud.Parte2.Trim());
            string str3 = Ajustar(solicitud.Parte3.Trim());
            string str4 = Ajustar(ConfiguracionGeneralSesion.CodUsuario.ToString());

            string strRes = Unir(str1, str2);
            strRes = Unir(strRes, str3);
            strRes = Unir(strRes, str4);

            int p = strRes.Length - 10;
            return new()
            {
                Parte1 = strRes.Substring(p, 3),
                Parte2 = strRes.Substring(p + 3, 3),
                Parte3 = strRes.Substring(p + 6, 3)
            };
        }

        private string Unir(string str1, string str2)
        {
            string resultado = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                int aux = (((str1[i] <= '9') ? Convert.ToInt32(str1[i]) - 48 : Convert.ToInt32(str1[i]) - 55)
                    + ((str2[i] <= '9') ? Convert.ToInt32(str2[i]) - 48 : Convert.ToInt32(str2[i]) - 55)) * (i + 1);
                while (aux >= 35)
                {
                    aux -= 35;
                }
                resultado += (aux <= 9) ? Convert.ToChar(aux + 48) : Convert.ToChar(aux + 55);
            }
            return resultado;
        }

        private string Ajustar(string texto)
        {
            while (texto.Length < 10)
            {
                texto += texto;
            }
            return texto[..10];
        }
    }
}
