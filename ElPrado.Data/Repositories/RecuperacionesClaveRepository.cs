using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class RecuperacionesClaveRepository : RepositoryBaseEntidad<RecuperacionesClave>
    {
        public RecuperacionesClaveRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task AgregarAsync(RecuperacionesClave recuperacion)
        {
            const string sql = @"INSERT INTO RECUPERACIONES_CLAVE
                (COD_CLIENTE, PROPUESTA, TOKEN_HASH, FECHA_CREACION, FECHA_VENCIMIENTO, IP_SOLICITUD)
                VALUES
                (@CodCliente, @Propuesta, @TokenHash, @FechaCreacion, @FechaVencimiento, @IpSolicitud)";
            await _connection.ExecuteAsync(sql, recuperacion, _transaction);
        }

        public async Task<RecuperacionesClave?> BuscarTokenActivoAsync(string tokenHash, DateTime ahora)
        {
            const string sql = @"SELECT * FROM RECUPERACIONES_CLAVE
                WHERE TOKEN_HASH = @tokenHash
                  AND FECHA_UTILIZACION IS NULL
                  AND FECHA_VENCIMIENTO > @ahora";
            return await _connection.QuerySingleOrDefaultAsync<RecuperacionesClave>(sql, new { tokenHash, ahora }, _transaction);
        }

        public async Task ConsumirAsync(int codRecuperacion, DateTime ahora)
        {
            const string sql = @"UPDATE RECUPERACIONES_CLAVE
                SET FECHA_UTILIZACION = @ahora
                WHERE COD_RECUPERACION = @codRecuperacion
                  AND FECHA_UTILIZACION IS NULL
                  AND FECHA_VENCIMIENTO > @ahora";
            await _connection.ExecuteAsync(sql, new { codRecuperacion, ahora }, _transaction);
        }

        public async Task InvalidarPendientesAsync(int codCliente, int codRecuperacionUtilizada, DateTime ahora)
        {
            const string sql = @"UPDATE RECUPERACIONES_CLAVE
                SET FECHA_UTILIZACION = @ahora
                WHERE COD_CLIENTE = @codCliente
                  AND COD_RECUPERACION <> @codRecuperacionUtilizada
                  AND FECHA_UTILIZACION IS NULL";
            await _connection.ExecuteAsync(sql, new { codCliente, codRecuperacionUtilizada, ahora }, _transaction);
        }

        public async Task<int> ContarPorPropuestaDesdeAsync(int propuesta, DateTime desde)
        {
            const string sql = @"SELECT COUNT(*) FROM RECUPERACIONES_CLAVE
                WHERE PROPUESTA = @propuesta AND FECHA_CREACION >= @desde";
            return await _connection.QuerySingleAsync<int>(sql, new { propuesta, desde }, _transaction);
        }

        public async Task<int> ContarPorIpDesdeAsync(string ipSolicitud, DateTime desde)
        {
            const string sql = @"SELECT COUNT(*) FROM RECUPERACIONES_CLAVE
                WHERE IP_SOLICITUD = @ipSolicitud AND FECHA_CREACION >= @desde";
            return await _connection.QuerySingleAsync<int>(sql, new { ipSolicitud, desde }, _transaction);
        }
    }
}
