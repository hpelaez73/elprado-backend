using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class ColaEnvioFacturaRepository : RepositoryBaseEntidad<ColaEnvioFactura>
    {
        public ColaEnvioFacturaRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ColaEnvioFactura>> BuscarPendientesAsync(int limite)
        {
            string sql = $@"SELECT FIRST {limite} *
                            FROM COLA_ENVIOS_FACTURAS
                            WHERE ESTADO IN ('Pendiente','Error')
                            AND INTENTOS < 5
                            AND POR_EMAIL = 1
                            ORDER BY FECHA_CREACION";

            return await _connection.QueryAsync<ColaEnvioFactura>(sql, null, _transaction);
        }

        public async Task MarcarEnviandoAsync(int codColaEnvio)
        {
            string sql = @" UPDATE COLA_ENVIOS_FACTURAS
                            SET ESTADO = 'Enviando',
                            FECHA_ULTIMO_INTENTO = CURRENT_TIMESTAMP
                            WHERE COD_COLA_ENVIO = @codColaEnvio";

            await _connection.ExecuteAsync(sql, new { codColaEnvio }, _transaction);
        }

        public async Task MarcarEnviadoAsync(int codColaEnvio)
        {
            string sql = @" UPDATE COLA_ENVIOS_FACTURAS
                            SET ESTADO = 'Enviado'
                            WHERE COD_COLA_ENVIO = @codColaEnvio";

            await _connection.ExecuteAsync(sql, new { codColaEnvio }, _transaction);
        }

        public async Task MarcarErrorAsync(int codColaEnvio, string error)
        {
            string sql = @" UPDATE COLA_ENVIOS_FACTURAS
                            SET ESTADO = 'Error',
                            INTENTOS = INTENTOS + 1,
                            ERROR = @error
                            WHERE COD_COLA_ENVIO = @codColaEnvio";

            await _connection.ExecuteAsync(sql, new { codColaEnvio, error }, _transaction);
        }
    }
}
