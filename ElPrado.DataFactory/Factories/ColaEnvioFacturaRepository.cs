using Dapper;
using ElPrado.Core.Domains;
using ElPrado.DataFactory.Interfaces;
using ElPrado.Dto.Dtos;

namespace ElPrado.DataFactory.Factories
{
    public class ColaEnvioFacturaRepository : IColaEnvioFacturaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ColaEnvioFacturaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<DtoColaEnvioFactura>> BuscarPendientesAsync()
        {
            string sql = $@"SELECT * FROM GET_COLA_ENVIO_FACTURAS";

            using var connection = _connectionFactory.Create();
            return await connection.QueryAsync<DtoColaEnvioFactura>(sql);
        }

        public async Task MarcarEnviandoAsync(int codColaEnvio)
        {
            string sql = "EXECUTE PROCEDURE PUT_COLA_ENVIO_FACTURAS(@codColaEnvio, @estado, NULL)";

            using var connection = _connectionFactory.Create();
            await connection.ExecuteAsync(sql, new { codColaEnvio, estado = EstadoEnvioDomain.Enviando });
        }

        public async Task MarcarEnviadoAsync(int codColaEnvio)
        {
            string sql = "EXECUTE PROCEDURE PUT_COLA_ENVIO_FACTURAS(@codColaEnvio, @estado, NULL)";

            using var connection = _connectionFactory.Create();
            await connection.ExecuteAsync(sql, new { codColaEnvio, estado = EstadoEnvioDomain.Enviado });
        }

        public async Task MarcarErrorAsync(int codColaEnvio, string error)
        {
            string sql = "EXECUTE PROCEDURE PUT_COLA_ENVIO_FACTURAS(@codColaEnvio, @estado, @error)";

            using var connection = _connectionFactory.Create();
            await connection.ExecuteAsync(sql, new { codColaEnvio, estado = EstadoEnvioDomain.Error, error });
        }
    }
}
