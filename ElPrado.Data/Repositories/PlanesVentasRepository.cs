using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class PlanesVentasRepository : RepositoryBaseEntidad<PlanesVentas>
    {
        public PlanesVentasRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public List<DtoPlanesVentasPropuestas> BuscarPlanesVentas(int codPropuesta, bool incluirBaja)
        {
            string sql = "SELECT * FROM GET_DATOS_DET_PLANES_VENTAS(@codPropuesta, @incluirBaja)";
            return connection.Query<DtoPlanesVentasPropuestas>(sql, new { codPropuesta, incluirBaja }, transaction).ToList();
        }
    }
}
