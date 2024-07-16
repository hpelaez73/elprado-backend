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
            return conexion.QuerySingleOrDefault<DtoCoordenadas>(sql, new { codParcela, nroTelefono }, transaccion);
        }

        public List<DtoParcelasDetallesLugares>? BuscarDetalleLugares(int codParcela, int codPropuesta)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELAS_NIVELES(@codParcela, @codPropuesta) P";
            return conexion.Query<DtoParcelasDetallesLugares>(sql, new { codParcela, codPropuesta }, transaccion).ToList();
        }

        public List<string>? BuscarZonasParcelas(int codParcela)
        {
            string sql = "SELECT * FROM GET_DATOS_PARCELAS_ZONAS(@codParcela) P";
            return conexion.Query<string>(sql, new { codParcela }, transaccion).ToList();
        }
    }
}
