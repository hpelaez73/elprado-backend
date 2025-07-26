using ElPrado.Core;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> values = new()
            {
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!,
                ConfiguracionGeneralSesion.StrConexion,
            };
            if (ConfiguracionGeneralSesion.AllowedOrigins != null)
                values.AddRange(ConfiguracionGeneralSesion.AllowedOrigins);
            values.Add("Version: 26/07/2025 14:10");
            return values;
        }
    }
}