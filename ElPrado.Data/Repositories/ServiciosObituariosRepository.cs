using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ServiciosObituariosRepository : RepositoryBaseCrud<ServiciosObituarios, DtoServiciosObituarios>
    {
        public ServiciosObituariosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override DtoServiciosObituarios? Visualizar(int id)
        {
            string sql = @" SELECT S.*, T.TIPO_SERVICIO
                            FROM SERVICIOS_OBITUARIOS S
                            INNER JOIN TIPOS_SERVICIOS T ON T.COD_TIPO_SERVICIO = S.COD_TIPO_SERVICIO
                            WHERE S.COD_SERVICIO = @id";
            return _connection.QueryFirstOrDefault<DtoServiciosObituarios>(sql, new { id }, _transaction);
        }

        public List<DtoServiciosObituarios> BuscarPorObituario(int id)
        {
            string sql = @" SELECT S.*, T.TIPO_SERVICIO
                            FROM SERVICIOS_OBITUARIOS S
                            INNER JOIN TIPOS_SERVICIOS T ON T.COD_TIPO_SERVICIO = S.COD_TIPO_SERVICIO
                            WHERE S.COD_OBITUARIO = @id";
            return _connection.Query<DtoServiciosObituarios>(sql, new { id }, _transaction).ToList();
        }
    }
}
