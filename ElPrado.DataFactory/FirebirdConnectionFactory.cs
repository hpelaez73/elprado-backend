using Dapper;
using ElPrado.DataFactory.Interfaces;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ElPrado.DataFactory
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
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            var conn = new FbConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
