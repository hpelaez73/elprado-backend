using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class ProcesosSistemasRepository : RepositoryBaseEntidad<ProcesosSistemas>
    {
        public ProcesosSistemasRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public void AgregarModificar(ProcesosSistemas procesosSistemas, string campoMatch)
        {
            // Pasar a DapperCrud
            string sql = $@"UPDATE OR INSERT INTO PROCESOS_SISTEMAS (PROCESO, NOMBRE, CATEGORIA, MODULO_WEB)
                            VALUES (@Proceso, @Nombre, @Categoria, @ModuloWeb)
                            MATCHING ({campoMatch})";
            _connection.Execute(sql, new { procesosSistemas.Proceso, procesosSistemas.Nombre, procesosSistemas.Categoria, procesosSistemas.ModuloWeb }, _transaction);
        }

        public void LimpiarProcesosWeb()
        {
            string sql = @" UPDATE PROCESOS_SISTEMAS P SET
                            P.MODULO_WEB = 0
                            WHERE P.MODULO_WEB = 1";
            _connection.Execute(sql, null, _transaction);
        }
    }
}
