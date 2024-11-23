using ElPrado.Core;
using FirebirdSql.Data.FirebirdClient;
using System.Data;

namespace ElPrado.Data
{
    internal static class Conexion
    {
        static Conexion()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public static IDbConnection CrearConexion()
        {
            FbConnection connection = new(ConfiguracionGeneralSesion.StrConexion);
            connection.Open();
            return connection;
        }
    }

    public sealed class Transaccion : IDisposable
    {
        private IDbConnection conexion;
        private IDbTransaction transaccion;
        private bool transaccionActiva = false;
        private Guid identificador = Guid.NewGuid();

        public Transaccion()
        {
            conexion = Conexion.CrearConexion();
            transaccion = conexion.BeginTransaction();
            transaccionActiva = true;
        }

        public void Commit()
        {
            transaccion.Commit();
            transaccionActiva = false;
        }

        public void Rollback()
        {
            transaccion.Rollback();
            transaccionActiva = false;
        }

        public void Dispose()
        {
            if (transaccionActiva) transaccion.Commit();
            conexion.Close();
            transaccion.Dispose();
            conexion.Dispose();
        }

        public IDbConnection GetConnection()
        {
            return conexion;
        }

        public IDbTransaction GetTransaction()
        {
            return transaccion;
        }

    }
}