using ElPrado.Core;
using System.Security.Claims;

namespace ElPrado.WebApi.MiddleWares
{
    public class LoginMiddleWare
    {
        private readonly RequestDelegate next;

        public LoginMiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var claimUsuario = context.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier));
            int codAcceso = (claimUsuario != null) ? Convert.ToInt32(claimUsuario.Value) : 0;
            var claimPropuesta = context.User.Claims.FirstOrDefault(x => x.Type.Equals("CodPropuesta"));
            int codPropuesta = (claimPropuesta != null) ? Convert.ToInt32(claimPropuesta.Value) : 0;

            ConfiguracionGeneralSesion.Inicializar(codAcceso, codPropuesta);

            await next.Invoke(context).ConfigureAwait(false);
        }
    }
}
