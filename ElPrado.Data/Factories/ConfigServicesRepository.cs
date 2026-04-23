using Dapper;
using ElPrado.Core.Configuration;
using ElPrado.Data.Interfaces;

namespace ElPrado.Data.Factories
{
    public class ConfigServicesRepository : IConfigServicesRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ConfigServicesRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public EmailSettings? BuscarConfigMail()
        {
            using var conn = _connectionFactory.Create();
            string sql = @" SELECT C.SMTP_HOST, C.SMTP_PORT, C.SMTP_AUTENTICACION, C.SMTP_USUARIO, C.SMTP_CLAVE, C.SMTP_USUARIO_COBRANZA, C.SMTP_CLAVE_COBRANZA
                            FROM CONFIGURACION_LOCAL C";
            return conn.QuerySingleOrDefault<EmailSettings>(sql);
        }
    }
}
