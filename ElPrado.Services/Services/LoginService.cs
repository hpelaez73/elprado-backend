using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;
using System.Security.Cryptography;

namespace ElPrado.Services.Services
{
    [Descripcion("Acceso al sistema por parte de un usuario")]
    public class LoginService : ServiceBase
    {
        public LoginService(Transaccion? transaccion) : base(transaccion)
        {
        }

        public DtoLogin? Login(int legajo, long dniCuit, string clave)
        {
            ConfiguracionGeneralRepository configuracionGeneralRepository = new(Transaccion);
            bool esAdmin = (configuracionGeneralRepository.BuscarClaveMaestra() == Utils.SHA1(clave));
            ClientesRepository clientesRepository = new(Transaccion);
            Clientes? cliente = clientesRepository.Buscar(legajo, dniCuit, Utils.SHA1(clave), esAdmin);
            if (cliente != null)
            {
                PropuestasRepository propuestasRepository = new(Transaccion);
                Propuestas? propuesta = propuestasRepository.BuscarPropuesta(legajo);
                if (propuesta != null)
                {
                    try
                    {
                        RegistrarLogCliente("Acceso desde la web", cliente.CodCliente);
                        DtoLogin dtoLogin = GenerarTokenCliente(cliente, propuesta);
                        
                        Commit();
                        return dtoLogin;
                    }
                    catch
                    {
                        Rollback();
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
            UsuariosRepository usuariosRepository = new(Transaccion);
            Usuarios? usuario = usuariosRepository.Buscar(alias, clave);
            if (usuario != null)
            {
                try
                { 
                    RegistrarLogUsuario("Acceso desde la web", usuario.CodUsuario);
                    DtoLogin dtoLogin = GenerarTokenUsuario(usuario);

                    Commit();
                    return dtoLogin;
                }
                catch
                {
                    Rollback();
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
            RefreshTokensRepository refreshTokensRepository = new(Transaccion);
            RefreshTokens? refreshToken = refreshTokensRepository.BuscarTokenActivo(token);

            if (refreshToken == null) return null;
            if (refreshToken.CodCliente != null && refreshToken.CodPropuesta != null)
            {
                ClientesRepository clientesRepository = new(Transaccion);
                PropuestasRepository propuestasRepository = new(Transaccion);
                Clientes cliente = clientesRepository.Buscar(refreshToken.CodCliente.Value);
                Propuestas propuesta = propuestasRepository.Buscar(refreshToken.CodPropuesta.Value);
                return GenerarTokenCliente(cliente, propuesta);
            }
            else if (refreshToken.CodUsuario != null)
            {
                UsuariosRepository usuariosRepository = new(Transaccion);
                Usuarios usuario = usuariosRepository.Buscar(refreshToken.CodUsuario.Value);
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

            ClientesRepository clientesRepository = new(Transaccion);
            Clientes? cliente = clientesRepository.Buscar(altaCliente.Propuesta, altaCliente.DniCuit);
            if (cliente == null)
            {
                resultado.Agregar("No hay cliente con los datos ingresados");
                return resultado;
            }

            try
            {
                cliente.ClaveAcceso = Utils.SHA1(altaCliente.Clave.Trim());
                cliente.Email = altaCliente.Email.Trim();
                clientesRepository.Modificar(cliente);
                
                Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            return resultado;
        }

        public Resultados BorrarCliente(int codCliente)
        {
            Resultados resultado = new();

            ClientesRepository clientesRepository = new(Transaccion);
            Clientes? cliente = clientesRepository.Buscar(codCliente);
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
                clientesRepository.Modificar(cliente);

                Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
            return resultado;
        }

        private string GenerarRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            string token = Convert.ToBase64String(tokenBytes);

            RefreshTokensRepository refreshTokensRepository = new(Transaccion);

            if (refreshTokensRepository.ExisteToken(token))
            {
                return GenerarRefreshToken();
            }
            return token;
        }

        private void GuardarRefreshToken(int? codCliente, int? codPropuesta, int? codUsuario, string refreshtoken)
        {
            RefreshTokensRepository refreshTokensRepository = new(Transaccion);
            RefreshTokens refreshToken = new()
            {
                CodCliente = codCliente,
                CodPropuesta = codPropuesta,
                CodUsuario = codUsuario,
                Token = refreshtoken,
                FechaExpiracion = DateTime.Today.AddDays(7)
            };

            refreshTokensRepository.Agregar(refreshToken);
        }
    }
}
