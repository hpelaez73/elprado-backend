using ElPrado.Services;
using System.Security.Claims;

namespace ElPrado.WebApi
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCodUsuario()
        {
            var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier));
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }

        public int GetCodCliente()
        {
            // Assuming CodCliente is stored in a claim. If not, this needs to be adjusted.
            var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.Equals("CodCliente"));
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }

        public int GetCodPropuesta()
        {
            var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.Equals("CodPropuesta"));
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }

        public bool EsTesting()
        {
            // Assuming there's a claim that indicates if the user is in testing mode.
            var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.Equals("EsTesting"));
            return (claim != null) && Convert.ToBoolean(claim.Value);
        }
    }
}
