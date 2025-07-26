using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto;

namespace ElPrado.Data.Repositories
{
    public class MovimientosFondosRepository : RepositoryBaseEntidad<MovimientosFondos>
    {
        public MovimientosFondosRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public DtoMovimientosFondos? Visualizar(int codMovimientoFondo)
        {
            string sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO(@codMovimientoFondo)";
            DtoMovimientosFondos? movimientoFondo = connection.QueryFirstOrDefault<DtoMovimientosFondos>(sql, new { codMovimientoFondo }, transaction);
            if (movimientoFondo != null)
            {
                sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO_CTAS(@codMovimientoFondo)";
                movimientoFondo.ListCuentas = connection.Query<DtoMovimientosFondosCuentas>(sql, new { codMovimientoFondo }, transaction).ToList();

                sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO_CUP(@codMovimientoFondo)";
                movimientoFondo.ListCupones = connection.Query<DtoMovimientosFondosCupones>(sql, new { codMovimientoFondo }, transaction).ToList();

                sql = "SELECT * FROM GET_DATOS_MOVIMIENTO_FONDO_CHEQ(@codMovimientoFondo)";
                movimientoFondo.ListCheques = connection.Query<DtoMovimientosFondosCheques>(sql, new { codMovimientoFondo }, transaction).ToList();
            }
            return movimientoFondo;
        }
    }
}
