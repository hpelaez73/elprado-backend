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

        public void Agregar(DetPreferenciasMercadopagoCr detPreferenciasMercadopagoCr)
        {
            conexion.Insert(detPreferenciasMercadopagoCr, transaccion);
        }

        public void Agregar(DetPreferenciasMercadopagoCp detPreferenciasMercadopagoCp)
        {
            conexion.Insert(detPreferenciasMercadopagoCp, transaccion);
        }

        public void ImputarPago(long id, int codReference, DateTime fechaPago)
        {
            string sql = "EXECUTE PROCEDURE IMPUTAR_MERCADOPAGO(@id, @codReference, @fechaPago)";

            conexion.Execute(sql, new {id, codReference, fechaPago}, transaccion);
        }
    }
}
