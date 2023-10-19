using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class PropuestasRepository : RepositoryBaseEntidad<Propuestas>
    {
        public PropuestasRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public Propuestas? BuscarPropuesta(int legajo)
        {
            List<Propuestas> listPropuestas = conexion.GetList<Propuestas>(new { Legajo = legajo }, transaccion).AsList();
            return listPropuestas.Count == 0 ? null : listPropuestas[0];
        }
    }
}
