using ElPrado.Core;
using ElPrado.Core.Utils;
using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;
using System.Security.Cryptography;

namespace ElPrado.Services.Services
{
    [Descripcion("Acceso al sistema por parte de un usuario")]
    public class LoginService : ServiceBase
    {
        public LoginService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public DtoLogin? Login(int legajo, long dniCuit, string clave)
        {
            string claveMaestra = _uow.ConfiguracionGeneral.BuscarClaveMaestra();
            string claveHash = FunUtils.SHA1(clave);
            bool esAdmin = claveMaestra.Equals(claveHash);

            Clientes? cliente = _uow.Clientes.Buscar(legajo, dniCuit, claveHash, esAdmin);
            if (cliente != null)
            {
                Propuestas? propuesta = _uow.Propuestas.BuscarPropuesta(legajo);
                if (propuesta != null)
                {
                    try
                    {
                        RegistrarLogCliente("Acceso desde la web", cliente.CodCliente);
                        DtoLogin dtoLogin = GenerarTokenCliente(cliente, propuesta);

                        _uow.Commit();
                        return dtoLogin;
                    }
                    catch
                    {
                        _uow.Rollback();
                        throw;
                    }
                }
                return null;
            }
            return null;
        }

        private DtoLogin GenerarTokenCliente(Clientes cliente, Propuestas propuesta)
        {
            DtoLogin dtoLogin = LogginMapper.MapToDto(cliente, propuesta);
            dtoLogin.RefreshToken = GenerarRefreshToken();
            GuardarRefreshToken(cliente.CodCliente, propuesta.CodPropuesta, null, dtoLogin.RefreshToken);
            return dtoLogin;
        }

        public DtoLogin? Login(string alias, string clave)
        {
            DtoLogin? dtoLogin = AutenticarUsuario(alias, clave, commit: false);
            if (dtoLogin == null) return null;

            try
            {
                dtoLogin.RefreshToken = GenerarRefreshToken();
                GuardarRefreshToken(null, null, dtoLogin.CodUsuario, dtoLogin.RefreshToken);
                _uow.Commit();
                return dtoLogin;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public DtoLogin? AutenticarUsuario(string alias, string clave)
        {
            return AutenticarUsuario(alias, clave, commit: true);
        }

        private DtoLogin? AutenticarUsuario(string alias, string clave, bool commit)
        {
            Usuarios? usuario = _uow.Usuarios.Buscar(alias, clave);
            if (usuario != null)
            {
                try
                {
                    RegistrarLogUsuario("Acceso desde la web", usuario.CodUsuario);
                    DtoLogin dtoLogin = LogginMapper.MapToDto(usuario);
                    if (commit) _uow.Commit();
                    return dtoLogin;
                }
                catch
                {
                    _uow.Rollback();
                    throw;
                }
            }
            return null;
        }

        public DtoLogin? RenovarToken(string token)
        {
            RefreshTokens? refreshToken = _uow.RefreshTokens.BuscarTokenActivo(token);

            if (refreshToken == null) return null;
            if (refreshToken.CodCliente != null && refreshToken.CodPropuesta != null)
            {
                Clientes cliente = _uow.Clientes.Buscar(refreshToken.CodCliente.Value);
                Propuestas propuesta = _uow.Propuestas.Buscar(refreshToken.CodPropuesta.Value);
                return GenerarTokenCliente(cliente, propuesta);
            }
            else if (refreshToken.CodUsuario != null)
            {
                Usuarios usuario = _uow.Usuarios.Buscar(refreshToken.CodUsuario.Value);
                DtoLogin dtoLogin = LogginMapper.MapToDto(usuario);
                dtoLogin.RefreshToken = GenerarRefreshToken();
                GuardarRefreshToken(null, null, usuario.CodUsuario, dtoLogin.RefreshToken);
                return dtoLogin;
            }
            return null;
        }

        public Resultados Registrar(DtoLoginClienteAlta altaCliente)
        {
            Resultados resultado = new();
            if (altaCliente.Clave.Trim() != altaCliente.ClaveConfirmacion.Trim()) resultado.Agregar("La clave de confimación no coincide");
            if (!FunUtils.EsMailValido(altaCliente.Email)) resultado.Agregar("La direccón de correo no es válida");
            resultado.Agregar(ValidarClave(altaCliente.Clave));

            if (resultado.HayError) return resultado;

            Clientes? cliente = _uow.Clientes.Buscar(altaCliente.Propuesta, altaCliente.DniCuit);
            if (cliente == null)
            {
                resultado.Agregar("No hay cliente con los datos ingresados");
                return resultado;
            }

            try
            {
                cliente.ClaveAcceso = HashearClave(altaCliente.Clave);
                cliente.Email = altaCliente.Email.Trim();
                cliente.TipoDocumento = (cliente.TipoDocumento == string.Empty) ? null : cliente.TipoDocumento;
                _uow.Clientes.Modificar(cliente);

                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
            return resultado;
        }

        public async Task<Resultados<DtoEmailRecuperacionClave>> SolicitarRecuperacionClienteAsync(DtoSolicitudRecuperacionCliente solicitud, string? ipSolicitud)
        {
            Resultados<DtoEmailRecuperacionClave> resultado = new();
            DateTime ahora = DateTime.Now;

            if (await _uow.RecuperacionesClave.ContarPorPropuestaDesdeAsync(solicitud.Propuesta, ahora.AddMinutes(-15)) >= 3 ||
                (!string.IsNullOrWhiteSpace(ipSolicitud) &&
                 await _uow.RecuperacionesClave.ContarPorIpDesdeAsync(ipSolicitud, ahora.AddHours(-1)) >= 10))
            {
                return resultado;
            }

            Clientes? cliente = await _uow.Clientes.BuscarParaRecuperacionAsync(solicitud.Propuesta, solicitud.DniCuit);
            if (cliente == null || string.IsNullOrWhiteSpace(cliente.ClaveAcceso) || !FunUtils.EsMailValido(cliente.Email ?? string.Empty))
            {
                return resultado;
            }

            string token = GenerarTokenRecuperacion();
            RecuperacionesClave recuperacion = new()
            {
                CodCliente = cliente.CodCliente,
                Propuesta = solicitud.Propuesta,
                TokenHash = HashearToken(token),
                FechaCreacion = ahora,
                FechaVencimiento = ahora.AddMinutes(30),
                IpSolicitud = ipSolicitud
            };

            try
            {
                await _uow.RecuperacionesClave.AgregarAsync(recuperacion);
                RegistrarLogCliente($"CLIENTE_PASSWORD_RESET_REQUESTED propuesta={solicitud.Propuesta} ip={ipSolicitud ?? "no-disponible"}", cliente.CodCliente);
                _uow.Commit();
                resultado.Valor = new DtoEmailRecuperacionClave
                {
                    Email = cliente.Email!,
                    Token = token
                };
            }
            catch
            {
                _uow.Rollback();
                throw;
            }

            return resultado;
        }

        public async Task<Resultados<DtoEmailAvisoClave>> RestablecerClaveClienteAsync(DtoRestablecerClaveCliente solicitud, string? ipSolicitud)
        {
            Resultados<DtoEmailAvisoClave> resultado = new();
            if (string.IsNullOrWhiteSpace(solicitud.Token))
            {
                resultado.Agregar("El token de recuperación es inválido o venció");
                return resultado;
            }

            DateTime ahora = DateTime.Now;
            RecuperacionesClave? recuperacion = await _uow.RecuperacionesClave.BuscarTokenActivoAsync(HashearToken(solicitud.Token), ahora);
            if (recuperacion == null)
            {
                resultado.Agregar("El token de recuperación es inválido o venció");
                return resultado;
            }

            resultado.Agregar(ValidarClave(solicitud.NuevaClave));
            if (resultado.HayError) return resultado;

            Clientes cliente = _uow.Clientes.Buscar(recuperacion.CodCliente);
            try
            {
                cliente.ClaveAcceso = HashearClave(solicitud.NuevaClave);

                await _uow.RecuperacionesClave.ConsumirAsync(recuperacion.CodRecuperacion, ahora);
                _uow.Clientes.Modificar(cliente);
                await _uow.RecuperacionesClave.InvalidarPendientesAsync(cliente.CodCliente, recuperacion.CodRecuperacion, ahora);
                RegistrarLogCliente($"CLIENTE_PASSWORD_RESET_COMPLETED propuesta={recuperacion.Propuesta} ip={ipSolicitud ?? "no-disponible"}", cliente.CodCliente);
                _uow.Commit();

                if (FunUtils.EsMailValido(cliente.Email ?? string.Empty))
                {
                    resultado.Valor = new DtoEmailAvisoClave { Email = cliente.Email! };
                }
            }
            catch
            {
                _uow.Rollback();
                throw;
            }

            return resultado;
        }

        private static Resultados ValidarClave(string? clave)
        {
            Resultados resultado = new();
            string claveNormalizada = clave?.Trim() ?? string.Empty;
            if (claveNormalizada == string.Empty) resultado.Agregar("La clave está vacia");
            if (claveNormalizada.Length < 6) resultado.Agregar("La clave debe tener al menos 6 caracteres");
            return resultado;
        }

        private static string HashearClave(string clave) => FunUtils.SHA1(clave.Trim());

        private static string GenerarTokenRecuperacion()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string HashearToken(string token)
        {
            byte[] hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash);
        }

        public Resultados BorrarCliente(int codCliente)
        {
            Resultados resultado = new();

            Clientes? cliente = _uow.Clientes.Buscar(codCliente);
            if (cliente == null)
            {
                resultado.Agregar("El cliente no existe");
                return resultado;
            }
            if (string.IsNullOrEmpty(cliente.ClaveAcceso)) resultado.Agregar("El cliente no está registrado en la Web");

            if (resultado.HayError) return resultado;

            try
            {
                cliente.ClaveAcceso = string.Empty;
                _uow.Clientes.Modificar(cliente);

                _uow.Commit();
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
            return resultado;
        }

        private string GenerarRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            string token = Convert.ToBase64String(tokenBytes);

            if (_uow.RefreshTokens.ExisteToken(token))
            {
                return GenerarRefreshToken();
            }
            return token;
        }

        private void GuardarRefreshToken(int? codCliente, int? codPropuesta, int? codUsuario, string refreshtoken)
        {
            RefreshTokens refreshToken = new()
            {
                CodCliente = codCliente,
                CodPropuesta = codPropuesta,
                CodUsuario = codUsuario,
                Token = refreshtoken,
                FechaExpiracion = DateTime.Today.AddDays(7)
            };

            _uow.RefreshTokens.Agregar(refreshToken);
        }
    }
}
