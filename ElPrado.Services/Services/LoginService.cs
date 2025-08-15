using ElPrado.Core;
using ElPrado.Data;
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
            string claveHash = Utils.SHA1(clave);
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
            Usuarios? usuario = _uow.Usuarios.Buscar(alias, clave);
            if (usuario != null)
            {
                try
                {
                    RegistrarLogUsuario("Acceso desde la web", usuario.CodUsuario);
                    DtoLogin dtoLogin = GenerarTokenUsuario(usuario);

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

        private DtoLogin GenerarTokenUsuario(Usuarios usuario)
        {
            DtoLogin dtoLogin = LogginMapper.MapToDto(usuario);
            dtoLogin.RefreshToken = GenerarRefreshToken();
            GuardarRefreshToken(null, null, usuario.CodUsuario, dtoLogin.RefreshToken);
            return dtoLogin;
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
                return GenerarTokenUsuario(usuario);
            }
            return null;
        }

        public Resultados Registrar(DtoLoginClienteAlta altaCliente)
        {
            Resultados resultado = new();
            if (altaCliente.Clave.Trim() == string.Empty) resultado.Agregar("La clave está vacia");
            if (altaCliente.Clave.Trim() != altaCliente.ClaveConfirmacion.Trim()) resultado.Agregar("La clave de confimación no coincide");
            if (!Utils.EsMailValido(altaCliente.Email)) resultado.Agregar("La direccón de correo no es válida");
            if (altaCliente.Clave.Trim().Length < 6) resultado.Agregar("La clave debe tener al menos 6 caracteres");

            if (resultado.HayError) return resultado;

            Clientes? cliente = _uow.Clientes.Buscar(altaCliente.Propuesta, altaCliente.DniCuit);
            if (cliente == null)
            {
                resultado.Agregar("No hay cliente con los datos ingresados");
                return resultado;
            }

            try
            {
                cliente.ClaveAcceso = Utils.SHA1(altaCliente.Clave.Trim());
                cliente.Email = altaCliente.Email.Trim();
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
