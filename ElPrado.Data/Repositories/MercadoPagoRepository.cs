using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class MercadoPagoRepository : RepositoryBase
    {
        public MercadoPagoRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public void Agregar(PreferenciasMercadopago preferenciasMercadopago)
        {
            conexion.Insert(preferenciasMercadopago, transaccion);
        }

        public void ImputarPago(long id, string externalReference, DateTime fechaPago)
        {
            string sql = "EXECUTE PROCEDURE IMPUTAR_MERCADOPAGO(@id, @externalReference, @fechaPago)";

            conexion.Execute(sql, new {id, externalReference, fechaPago}, transaccion);
        }
    }
}
