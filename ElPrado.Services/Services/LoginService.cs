using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class LoginService : ServiceBase
    {
        public LoginService(Transaccion? transaccion) : base(transaccion)
        {
        }

        public DtoLogin? Login(int legajo, long dniCuit, string clave)
        {
            ClientesRepository clientesRepository = new(transaccion);
            Clientes? cliente = clientesRepository.Buscar(legajo, dniCuit, Utils.SHA1(clave));
            if (cliente != null)
            {
                PropuestasRepository propuestasRepository = new(transaccion);
                Propuestas? propuesta = propuestasRepository.BuscarPropuesta(legajo);
                if (propuesta != null)
                {
                    return LogginMapper.MapToDto(cliente, propuesta);
                }
            }
            return null;
        }

        public DtoLogin? Login(string alias, string clave)
        {
            UsuariosRepository usuariosRepository = new(transaccion);

            Usuarios? usuario = usuariosRepository.Buscar(alias, clave);
            return LogginMapper.MapToDto(usuario);
        }

        public Resultados Registrar(DtoLoginClienteAlta altaCliente)
        {
            Resultados resultado = new();
            if (altaCliente.Clave.Trim() == string.Empty) resultado.Agregar("La clave está vacia");
            if (altaCliente.Clave.Trim() != altaCliente.ClaveConfirmacion.Trim()) resultado.Agregar("La clave de confimación no coincide");
            if (!Utils.EsMailValido(altaCliente.Email)) resultado.Agregar("La direccón de correo no es válida");
            if (altaCliente.Clave.Trim().Length < 6) resultado.Agregar("La clave debe tener al menos 6 caracteres");

            if (resultado.HayError) return resultado;

            ClientesRepository clientesRepository = new(transaccion);
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
                
                transaccion.Commit();
            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
            return resultado;
        }
    }
}
