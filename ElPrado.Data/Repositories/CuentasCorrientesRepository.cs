using Dapper;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class CuentasCorrientesRepository : RepositoryBase
    {
        public CuentasCorrientesRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public List<DtoCuentasCorrientes> ConsultaDeudaMercadoPago(int codCliente, DateTime fechaDia)
        {
            string sql = "SELECT * FROM CONSULTA_DEUDA_MERCADOPAGO(@codCliente, @fechaDia)";
            return connection.Query<DtoCuentasCorrientes>(sql, new { codCliente, fechaDia }, transaction).AsList();
        }
    }
}
