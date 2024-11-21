using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class RefreshTokensRepository : RepositoryBaseEntidad<RefreshTokens>
    {
        public RefreshTokensRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public RefreshTokens? BuscarTokenActivo(string token)
        {
            string sql = @" SELECT *
                            FROM REFRESH_TOKENS R
                            WHERE R.TOKEN = @token
                            AND R.FECHA_EXPIRACION >= CURRENT_DATE";
            return connection.QuerySingleOrDefault<RefreshTokens?>(sql, new { token }, transaction);
        }

        public bool ExisteToken(string token)
        {
            return Existe(nameof(RefreshTokens), nameof(RefreshTokens.Token), token);
        }
    }
}
