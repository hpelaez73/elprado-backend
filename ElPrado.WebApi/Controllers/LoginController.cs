using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
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
        private readonly IUnitOfWork _uow;
        private readonly IUserContextService _userContext;
        private readonly IRecuperacionClaveEmailService _recuperacionClaveEmail;

        public LoginController(IConfiguration configuration, IUnitOfWork unitOfWork, IUserContextService userContext, IRecuperacionClaveEmailService recuperacionClaveEmail)
        {
            this.configuration = configuration;
            _uow = unitOfWork;
            _userContext = userContext;
            _recuperacionClaveEmail = recuperacionClaveEmail;
        }

        [HttpPost("Usuario")]
        public ActionResult<ApiResponse<DtoLogin>> LoginUsuario([FromBody] DtoLoginUsuario alta)
        {
            using LoginService loginService = new(_uow, _userContext);
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

        [HttpPost("Cliente")]
        public ActionResult<ApiResponse<DtoLogin>> LoginCliente([FromBody] DtoLoginCliente alta)
        {
            using LoginService loginService = new(_uow, _userContext);
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

        [HttpPost("RenovarToken")]
        public ActionResult<ApiResponse<DtoLogin>> RenovarToken([FromBody] DtoRefreshToken alta)
        {
            using LoginService loginService = new(_uow, _userContext);
            ApiResponse<DtoLogin> apiResponse = new();
            DtoLogin? login = loginService.RenovarToken(alta.RefreshToken);
            if (login != null)
            {
                login.Token = BuildToken(login);
                apiResponse.Data = login;
                return apiResponse;
            }
            else
            {
                apiResponse.Agregar("Refresh token inválido");
                return Unauthorized(apiResponse);
            }
        }

        [HttpPost("Registrar")]
        public ActionResult<ApiResponse<int>> RegistrarCliente([FromBody] DtoLoginClienteAlta altaCliente)
        {
            using LoginService loginService = new(_uow, _userContext);
            ApiResponse<int> apiResponse = new();
            Resultados resultado = loginService.Registrar(altaCliente);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Message = "Usuario creado exitosamente";
            return apiResponse;
        }

        [HttpPost("SolicitarRecuperacionCliente")]
        public async Task<ActionResult<ApiResponse<string>>> SolicitarRecuperacionCliente([FromBody] DtoSolicitudRecuperacionCliente solicitud)
        {
            const string mensajeGenerico = "Si los datos corresponden a una cuenta recuperable, recibirás un correo con las instrucciones.";
            ApiResponse<string> apiResponse = new()
            {
                Message = mensajeGenerico
            };
            if (solicitud.Propuesta <= 0 || solicitud.DniCuit <= 0) return BadRequest(apiResponse);

            using LoginService loginService = new(_uow, _userContext);
            string? ipSolicitud = HttpContext.Connection.RemoteIpAddress?.ToString();
            Resultados<DtoEmailRecuperacionClave> resultado = await loginService.SolicitarRecuperacionClienteAsync(solicitud, ipSolicitud);
            if (resultado.Valor != null)
            {
                await _recuperacionClaveEmail.EnviarEnlaceAsync(resultado.Valor.Email, resultado.Valor.Token);
            }
            return apiResponse;
        }

        [HttpPost("RestablecerClaveCliente")]
        public async Task<ActionResult<ApiResponse<string>>> RestablecerClaveCliente([FromBody] DtoRestablecerClaveCliente solicitud)
        {
            using LoginService loginService = new(_uow, _userContext);
            ApiResponse<string> apiResponse = new();
            Resultados<DtoEmailAvisoClave> resultado = await loginService.RestablecerClaveClienteAsync(solicitud, HttpContext.Connection.RemoteIpAddress?.ToString());
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }

            if (resultado.Valor != null) await _recuperacionClaveEmail.EnviarAvisoAsync(resultado.Valor.Email);
            apiResponse.Message = "La clave fue restablecida correctamente.";
            return apiResponse;
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<int>> BorrarCliente(int id)
        {
            using LoginService loginService = new(_uow, _userContext);
            ApiResponse<int> apiResponse = new();
            Resultados resultado = loginService.BorrarCliente(id);
            if (resultado.HayError)
            {
                apiResponse.Agregar(resultado);
                return BadRequest(apiResponse);
            }
            apiResponse.Data = id;
            apiResponse.Message = "Usuario eliminado";
            return apiResponse;
        }

        private string BuildToken(DtoLogin login)
        {
            bool esTesting = configuration.GetConnectionString("DefaultConnection")?.ToLower().Contains("elprado_dev") ?? false;
            // CREAMOS EL HEADER //
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(creds);

            // CREAMOS LOS CLAIMS //
            var listClaim = new[] {
                new Claim(ClaimTypes.NameIdentifier, login.CodUsuario.ToString()),
                new Claim(ClaimTypes.Name, login.Nombre),
                new Claim("CodCliente", login.CodCliente.ToString()),
                new Claim("CodPropuesta", login.CodPropuesta.ToString()),
                new Claim("EsTesting", esTesting.ToString()),
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
