using ElPrado.Data.Interfaces;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ElPrado.Data
{
    public class FirebirdConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public FirebirdConnectionFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection Create()
        {
            var conn = new FbConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
