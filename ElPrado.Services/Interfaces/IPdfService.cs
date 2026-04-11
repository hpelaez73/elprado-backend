using ElPrado.Dto.Documents;

namespace ElPrado.Services.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerarFactura(DocFactura factura);
        byte[] GenerarFacturas(DocFacturasList facturas);
    }
}
