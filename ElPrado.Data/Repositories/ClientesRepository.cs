using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ClientesRepository : RepositoryBaseCrud<Clientes, DtoClientes>
    {
        public ClientesRepository(DbContext dbContext) : base(dbContext)
        {            
        }

        public override DtoClientes? Visualizar(int id)
        {
            string sql = "SELECT * FROM GET_DATOS_CLIENTE(@id)";
            DtoClientes? cliente = _connection.QuerySingleOrDefault<DtoClientes>(sql, new { id }, _transaction);

            if (cliente != null)
            {
                DomiciliosRepository domiciliosRepository = new(_dbContext);
                if (cliente.CodDomicilioParticular != null) cliente.DomicilioParticular = domiciliosRepository.Visualizar(cliente.CodDomicilioParticular.Value);
                if (cliente.CodDomicilioLaboral != null) cliente.DomicilioLaboral = domiciliosRepository.Visualizar(cliente.CodDomicilioLaboral.Value);
                if (cliente.CodDomicilioCobranza != null) cliente.DomicilioCobranza = domiciliosRepository.Visualizar(cliente.CodDomicilioCobranza.Value);
            }

            return cliente;
        }

        public Clientes? Buscar(int legajo, long dniCuit, string clave, bool esAdmin)
        {
            long dni = dniCuit.ToString().Length <= 8 ? dniCuit : 0;
            long cuit = dniCuit.ToString().Length > 8 ? dniCuit : 0;
            string sql = @"SELECT C.*
                FROM CLIENTES C
                WHERE (C.NRO_DOCUMENTO = @dni OR C.CUIT = @cuit)
                AND (C.CLAVE_ACCESO = @clave OR @esAdmin = 1)
                AND C.FECHA_BAJA IS NULL AND C.FECHA_FALLECIMIENTO IS NULL
                AND EXISTS (SELECT 1 FROM PROPUESTA P
                INNER JOIN PROPUESTAS_TITULARES PT ON PT.COD_PROPUESTA = P.COD_PROPUESTA
                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                WHERE PT.FECHA_BAJA IS NULL AND P.FECHA_BAJA IS NULL AND ED.ACTIVA = 1
                AND P.LEGAJO = @legajo)";
            return _connection.QuerySingleOrDefault<Clientes?>(sql, new { dni, cuit, clave, legajo, esAdmin }, _transaction);
        }

        public Clientes? Buscar(int legajo, long dniCuit)
        {
            string sql = @"SELECT * FROM GET_DATOS_TITULAR_DNI_CUIT(@legajo, @dniCuit)";
            return _connection.QuerySingleOrDefault<Clientes?>(sql, new { dniCuit, legajo }, _transaction);
        }

        public List<DtoClientesPropuestas> BuscarTitulares(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_TITULARES(@codPropuesta)";
            return _connection.Query<DtoClientesPropuestas>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoClientesPropuestasFacturasPagos>? BuscarTitularesFacturasPagos(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_TITULARES_FACT_PAGOS(@codPropuesta)";
            return _connection.Query<DtoClientesPropuestasFacturasPagos>(sql, new { codPropuesta }, _transaction).ToList();
        }
    }
}
