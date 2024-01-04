using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ComprobantesRepository : RepositoryBaseEntidad<Comprobantes>
    {
        public ComprobantesRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public IEnumerable<DtoComprobantesFacturasElectronicas> Facturas(int codCliente, DateTime fechaDesde, DateTime fechaHasta)
        {
            string sql = "SELECT * FROM GET_FACTURAS_AFIP(@codCliente, @fechaDesde, @fechaHasta)";
            return conexion.Query<DtoComprobantesFacturasElectronicas>(sql, new { codCliente , fechaDesde, fechaHasta }, transaccion);
        }

        public DtoComprobantesPeriodo? PeriodosFacturacion(int codCliente)
        {
            string sql = "SELECT * FROM GET_PERIODOS_FACTURAS_AFIP(@codCliente)";
            return conexion.QuerySingleOrDefault<DtoComprobantesPeriodo?>(sql, new { codCliente }, transaccion);
        }
    }
}
