using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class MercadoPagoRepository : RepositoryBase
    {
        public MercadoPagoRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public void Agregar(PreferenciasMercadopago preferenciasMercadopago)
        {
            _connection.Insert(preferenciasMercadopago, _transaction);
        }

        public void Agregar(DetPreferenciasMercadopagoCr detPreferenciasMercadopagoCr)
        {
            _connection.Insert(detPreferenciasMercadopagoCr, _transaction);
        }

        public void Agregar(DetPreferenciasMercadopagoCp detPreferenciasMercadopagoCp)
        {
            _connection.Insert(detPreferenciasMercadopagoCp, _transaction);
        }

        public void CrearPreferencia(string referenciaExterna)
        {
            string sql = "EXECUTE PROCEDURE GUARDAR_PREFERENCIA_MERCADOPAGO(@idPreferencia, @referenciaExterna, NULL)";
            string idPreferencia = referenciaExterna.Length > 100 ? referenciaExterna[..99] : referenciaExterna;
            _connection.Execute(sql, new { idPreferencia, referenciaExterna }, _transaction);
        }

        public void ImputarPago(long id, int codReference, string referenciaExterna, DateTime fechaPago)
        {
            string sql = "EXECUTE PROCEDURE IMPUTAR_MERCADOPAGO(@id, @codReference, @referenciaExterna, @fechaPago)";

            _connection.Execute(sql, new {id, codReference, referenciaExterna, fechaPago }, _transaction);
        }
    }
}
