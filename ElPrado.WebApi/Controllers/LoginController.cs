using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ElPrado.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class LoginController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public LoginController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("Usuario")]
        public ActionResult<ApiResponse<DtoLogin>> LoginUsuario([FromBody] DtoLoginUsuario alta)
        {
            try
            {
                using LoginService loginService = new(null);
                ApiResponse<DtoLogin> apiResponse = new();
                DtoLogin? login = loginService.Login(alta.Alias, alta.Clave);
                if (login != null)
                {
                    login.Token = BuildToken(login);
                    apiResponse.Data = login;
                    return apiResponse;
                }
                else
                {
                    apiResponse.Agregar("Clave inválida o usuario inexistente");
                    return Unauthorized(apiResponse);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Login({@alta}): {Mensaje}", this, alta, ex.Message);
                throw;
            }
        }

        [HttpPost("Cliente")]
        public ActionResult<ApiResponse<DtoLogin>> LoginCliente([FromBody] DtoLoginCliente alta)
        {
            try
            {
                using LoginService loginService = new(null);
                ApiResponse<DtoLogin> apiResponse = new();
                DtoLogin? login = loginService.Login(alta.Propuesta, alta.DniCuit, alta.Clave);
                if (login != null)
                {
                    login.Token = BuildToken(login);
                    apiResponse.Data = login;
                    return apiResponse;
                }
                else
                {
                    apiResponse.Agregar("Clave inválida o cliente inexistente");
                    return Unauthorized(apiResponse);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Login({@alta}): {Mensaje}", this, alta, ex.Message);
                throw;
            }
        }

        [HttpPost("Registrar")]
        public ActionResult<ApiResponse<int>> RegistrarCliente([FromBody] DtoLoginClienteAlta altaCliente)
        {
            try
            {
                using LoginService loginService = new(null);
                ApiResponse<int> apiResponse = new();
                Resultados resultado = loginService.Registrar(altaCliente);
                if (resultado.HayError)
                {
                    apiResponse.Agregar(resultado);
                    return BadRequest(apiResponse);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.Registrar({@altaCliente}): {Mensaje}", this, altaCliente, ex.Message);
                throw;
            }
        }

        private string BuildToken(DtoLogin login)
        {
            // CREAMOS EL HEADER //
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(creds);

            // CREAMOS LOS CLAIMS //
            var listClaim = new[] {
                new Claim(ClaimTypes.NameIdentifier, (login.CodUsuario > 0) ? login.CodUsuario.ToString() : login.CodCliente.ToString()),
                new Claim(ClaimTypes.Name, login.Nombre),
                new Claim("CodPropuesta", login.CodPropuesta.ToString())
            };

            // CREAMOS EL PAYLOAD //
            var payload = new JwtPayload(
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Issuer"],
                    claims: listClaim,
                    notBefore: DateTime.Now,
                    // Expira a la 24 horas.
                    expires: DateTime.Now.AddHours(24)
                );

            // GENERAMOS EL TOKEN //
            var _Token = new JwtSecurityToken(
                    header,
                    payload
                );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }
    }
}
