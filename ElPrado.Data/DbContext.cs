using Dapper;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ElPrado.Data
{
    public sealed class DbContext : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private bool transaccionActiva = false;

        public DbContext(IConfiguration configuration)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
            SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());

            _connection = new FbConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();

            _transaction = _connection.BeginTransaction();
            transaccionActiva = true;
        }

        public void Commit()
        {
            _transaction.Commit();
            transaccionActiva = false;
        }

        public void Rollback()
        {
            if (transaccionActiva) _transaction.Rollback();
            transaccionActiva = false;
        }

        public void Dispose()
        {
            if (transaccionActiva) _transaction.Commit();
            _connection.Close();
            _transaction.Dispose();
            _connection.Dispose();
        }

        public IDbConnection GetConnection()
        {
            return _connection;
        }

        public IDbTransaction GetTransaction()
        {
            return _transaction;
        }

    }
}