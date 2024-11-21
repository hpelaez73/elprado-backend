using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ParcelasRepository : RepositoryBaseEntidad<Parcelas>
    {
        public ParcelasRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public DtoCoordenadas? BuscarCoordenadas(int codParcela, string nroTelefono)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELA_COORDENADA(@codParcela, @nroTelefono) P";
            return connection.QuerySingleOrDefault<DtoCoordenadas>(sql, new { codParcela, nroTelefono }, transaction);
        }

        public List<DtoParcelasDetallesLugares>? BuscarDetalleLugares(int codParcela, int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELAS_NIVELES(@codParcela, @codPropuesta) P";
            return connection.Query<DtoParcelasDetallesLugares>(sql, new { codParcela, codPropuesta }, transaction).ToList();
        }

        public List<string>? BuscarZonasParcelas(int codParcela)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELAS_ZONAS(@codParcela) P";
            return connection.Query<string>(sql, new { codParcela }, transaction).ToList();
        }
    }
}
