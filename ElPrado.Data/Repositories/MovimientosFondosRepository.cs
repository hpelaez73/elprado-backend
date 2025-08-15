using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto;

namespace ElPrado.Data.Repositories
{
    public class MovimientosFondosRepository : RepositoryBaseEntidad<MovimientosFondos>
    {
        public MovimientosFondosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public DtoMovimientosFondos? Visualizar(int codMovimientoFondo)
        {
            string sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO(@codMovimientoFondo)";
            DtoMovimientosFondos? movimientoFondo = _connection.QueryFirstOrDefault<DtoMovimientosFondos>(sql, new { codMovimientoFondo }, _transaction);
            if (movimientoFondo != null)
            {
                sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO_CTAS(@codMovimientoFondo)";
                movimientoFondo.ListCuentas = _connection.Query<DtoMovimientosFondosCuentas>(sql, new { codMovimientoFondo }, _transaction).ToList();

                sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO_CUP(@codMovimientoFondo)";
                movimientoFondo.ListCupones = _connection.Query<DtoMovimientosFondosCupones>(sql, new { codMovimientoFondo }, _transaction).ToList();

                sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO_CHEQ(@codMovimientoFondo)";
                movimientoFondo.ListCheques = _connection.Query<DtoMovimientosFondosCheques>(sql, new { codMovimientoFondo }, _transaction).ToList();
            }
            return movimientoFondo;
        }
    }
}
