using ElPrado.Dto.Documents;
using ElPrado.Reports.Documents;
using ElPrado.Services.Interfaces;
using QuestPDF.Fluent;

namespace ElPrado.Services.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerarFactura(DocFactura factura)
        {
            var doc = new FacturasDocument(
                new DocFacturasList
                {
                    Facturas = new() { factura }
                });

            return doc.GeneratePdf();
        }

        public byte[] GenerarFacturas(DocFacturasList facturas)
        {
            var doc = new FacturasDocument(facturas);

            return doc.GeneratePdf();
        }
    }
}
