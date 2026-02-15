using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ObituariosRepository : RepositoryBaseCrud<Obituarios, DtoObituarios>
    {
        public ObituariosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override DtoObituarios? Visualizar(int id)
        {
            string sql = "SELECT * FROM OBITUARIOS WHERE COD_OBITUARIO = @id";
            return _connection.QuerySingleOrDefault<DtoObituarios>(sql, new { id }, _transaction);
        }

        public DtoObituarioDetalleResp? BuscarDetalle(int id)
        {
            string sql = "SELECT * FROM OBITUARIOS WHERE COD_OBITUARIO = @id";
            return _connection.QuerySingleOrDefault<DtoObituarioDetalleResp>(sql, new { id }, _transaction);
        }

        public List<DtoReaccionesObituarios> BuscarReacciones(int id)
        {
            string sql = "SELECT * FROM REACCIONES_OBITUARIOS R WHERE R.COD_OBITUARIO = @id";
            return _connection.Query<DtoReaccionesObituarios>(sql, new { id }, _transaction).ToList();
        }

        public List<DtoServicioObituarios> BuscarServicios(int id)
        {
            string sql = @" SELECT S.*, T.TIPO_SERVICIO
                            FROM SERVICIOS_OBITUARIOS S
                            INNER JOIN TIPOS_SERVICIOS T ON T.COD_TIPO_SERVICIO = S.COD_TIPO_SERVICIO
                            WHERE S.COD_OBITUARIO = @id";
            return _connection.Query<DtoServicioObituarios>(sql, new { id }, _transaction).ToList();
        }
    }
}
