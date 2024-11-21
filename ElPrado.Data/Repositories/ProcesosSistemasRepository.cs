using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class ProcesosSistemasRepository : RepositoryBaseEntidad<ProcesosSistemas>
    {
        public ProcesosSistemasRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public void AgregarModificar(ProcesosSistemas procesosSistemas, string campoMatch)
        {
            // Pasar a DapperCrud
            string sql = $@"UPDATE OR INSERT INTO PROCESOS_SISTEMAS (PROCESO, NOMBRE, CATEGORIA, MODULO_WEB)
                            VALUES (@Proceso, @Nombre, @Categoria, @ModuloWeb)
                            MATCHING ({campoMatch})";
            connection.Execute(sql, new { procesosSistemas.Proceso, procesosSistemas.Nombre, procesosSistemas.Categoria, procesosSistemas.ModuloWeb }, transaction);
        }

        public void LimpiarProcesosWeb()
        {
            string sql = @" UPDATE PROCESOS_SISTEMAS P SET
                            P.MODULO_WEB = 0
                            WHERE P.MODULO_WEB = 1";
            connection.Execute(sql, null, transaction);
        }
    }
}
