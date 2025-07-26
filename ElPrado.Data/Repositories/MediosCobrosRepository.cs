using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class MediosCobrosRepository : RepositoryBaseCrud<MediosCobros, DtoMediosCobros>
    {
        public MediosCobrosRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public override DtoMediosCobros? Visualizar(int id)
        {
            string sql = "SELECT * FROM GET_DATOS_MEDIO_COBRO(@id)";
            return connection.QuerySingleOrDefault<DtoMediosCobros>(sql, new { id }, transaction);
        }

        public List<DtoHistorialCobradores> BuscarHistorialCobradores(int? codCredito, int? codConfiguracion)
        {
            string sql = "SELECT * FROM GET_HISTORIAL_COBRADORES(@codCredito, @codConfiguracion)";
            return connection.Query<DtoHistorialCobradores>(sql, new { codCredito, codConfiguracion }, transaction).ToList();
        }

    }
}
