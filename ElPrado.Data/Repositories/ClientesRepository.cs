using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ClientesRepository : RepositoryBaseCrud<Clientes, DtoClientes>
    {
        public ClientesRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public override DtoClientes? Visualizar(int id)
        {
            string sql = "SELECT * FROM CLIENTES C WHERE C.COD_CLIENTE = @id";
            return conexion.QuerySingleOrDefault<DtoClientes>(sql, new { id }, transaccion); 
        }

        public Clientes? Buscar(int legajo, long dniCuit, string clave, bool esAdmin)
        {
            string sql = @"SELECT C.*
                FROM CLIENTES C
                WHERE (C.NRO_DOCUMENTO = @dniCuit OR C.CUIT = @dniCuit)
                AND (C.CLAVE_ACCESO = @clave OR @esAdmin = 1)
                AND C.FECHA_BAJA IS NULL AND C.FECHA_FALLECIMIENTO IS NULL
                AND EXISTS (SELECT 1 FROM PROPUESTA P
                INNER JOIN PROPUESTAS_TITULARES PT ON PT.COD_PROPUESTA = P.COD_PROPUESTA
                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                WHERE PT.FECHA_BAJA IS NULL AND P.FECHA_BAJA IS NULL AND ED.ACTIVA = 1
                AND P.LEGAJO = @legajo)";
            return conexion.QuerySingleOrDefault<Clientes?>(sql, new { dniCuit, clave, legajo, esAdmin }, transaccion);
        }

        public Clientes? Buscar(int legajo, long dniCuit)
        {
            string sql = @"SELECT C.*
                FROM CLIENTES C
                WHERE (C.NRO_DOCUMENTO = @dniCuit OR C.CUIT = @dniCuit)
                AND C.FECHA_BAJA IS NULL AND C.FECHA_FALLECIMIENTO IS NULL
                AND EXISTS (SELECT 1 FROM PROPUESTA P
                INNER JOIN PROPUESTAS_TITULARES PT ON PT.COD_PROPUESTA = P.COD_PROPUESTA
                INNER JOIN ESTADOS_DEUDAS ED ON ED.COD_ESTADO_DEUDA = P.COD_ESTADO_DEUDA
                WHERE PT.FECHA_BAJA IS NULL AND P.FECHA_BAJA IS NULL AND ED.ACTIVA = 1
                AND P.LEGAJO = @legajo)";
            return conexion.QuerySingleOrDefault<Clientes?>(sql, new { dniCuit, legajo }, transaccion);
        }

        public List<DtoClientesPropuestas> BuscarTitulares(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_TITULARES(@codPropuesta)";
            return conexion.Query<DtoClientesPropuestas>(sql, new { codPropuesta }, transaccion).ToList();
        }

        public List<DtoClientesPropuestasFacturasPagos>? BuscarTitularesFacturasPagos(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_TITULARES_FACT_PAGOS(@codPropuesta)";
            return conexion.Query<DtoClientesPropuestasFacturasPagos>(sql, new { codPropuesta }, transaccion).ToList();
        }
    }
}
