using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class CreditosRepository : RepositoryBaseEntidad<Creditos>
    {
        public CreditosRepository(Transaccion transaccion) : base(transaccion)
        {
        }
    }
}
