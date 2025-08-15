using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> values = new()
            {
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!,
                _configuration.GetConnectionString("DefaultConnection") ?? "El string de conexión no está configurado"
            };
            values.Add("Version: 14/08/2025 21:10");
            return values;
        }
    }
}