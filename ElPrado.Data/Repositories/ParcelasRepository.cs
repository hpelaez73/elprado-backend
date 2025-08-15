using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ParcelasRepository : RepositoryBaseEntidad<Parcelas>
    {
        public ParcelasRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public DtoCoordenadas? BuscarCoordenadas(int codParcela, string nroTelefono)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELA_COORDENADA(@codParcela, @nroTelefono) P";
            return _connection.QuerySingleOrDefault<DtoCoordenadas>(sql, new { codParcela, nroTelefono }, _transaction);
        }

        public List<DtoParcelasDetallesLugares>? BuscarDetalleLugares(int codParcela, int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELAS_NIVELES(@codParcela, @codPropuesta) P";
            return _connection.Query<DtoParcelasDetallesLugares>(sql, new { codParcela, codPropuesta }, _transaction).ToList();
        }

        public List<string>? BuscarZonasParcelas(int codParcela)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELAS_ZONAS(@codParcela) P";
            return _connection.Query<string>(sql, new { codParcela }, _transaction).ToList();
        }
    }
}
