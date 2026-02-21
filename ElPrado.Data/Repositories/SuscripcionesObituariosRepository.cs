using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class SuscripcionesObituariosRepository : RepositoryBaseCrud<SuscripcionesObituarios, DtoSuscripcionesObituarios>
    {
        public SuscripcionesObituariosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override DtoSuscripcionesObituarios? Visualizar(int codSuscripcion)
        {
            string sql = @"SELECT *
                           FROM SUSCRIPCIONES_OBITUARIOS
                           WHERE COD_SUSCRIPCION = @codSuscripcion";
            return _connection.QuerySingleOrDefault<DtoSuscripcionesObituarios>(sql, new { codSuscripcion }, _transaction);
        }
    }
}