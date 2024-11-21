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
            connection.Insert(preferenciasMercadopago, transaction);
        }

        public void Agregar(DetPreferenciasMercadopagoCr detPreferenciasMercadopagoCr)
        {
            connection.Insert(detPreferenciasMercadopagoCr, transaction);
        }

        public void Agregar(DetPreferenciasMercadopagoCp detPreferenciasMercadopagoCp)
        {
            connection.Insert(detPreferenciasMercadopagoCp, transaction);
        }

        public void ImputarPago(long id, int codReference, DateTime fechaPago)
        {
            string sql = "EXECUTE PROCEDURE IMPUTAR_MERCADOPAGO(@id, @codReference, @fechaPago)";

            connection.Execute(sql, new {id, codReference, fechaPago}, transaction);
        }
    }
}
