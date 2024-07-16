using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class InhumadosRepository : RepositoryBaseEntidad<Inhumados>
    {
        public InhumadosRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public DtoInhumacion? BuscarInhumacion(int id)
        {
            string sql = "SELECT * FROM GET_DATOS_INHUMADO(@id)";
            return conexion.QuerySingleOrDefault<DtoInhumacion>(sql, new { id }, transaccion);
        }

        public List<DtoInhumadosPropuestas> BuscarInhumados(int codPropuesta, int codParcela)
        {
            string sql = "SELECT * FROM GET_DATOS_INHUMADOS(@codPropuesta, @codParcela)";
            return conexion.Query<DtoInhumadosPropuestas>(sql, new { codPropuesta, codParcela }, transaccion).ToList();
        }
    }
}
