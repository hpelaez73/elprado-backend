using ElPrado.Dto.Documents;
using System.Text;
using System.Text.Json;

namespace ElPrado.Reports.Helpers
{
    public static class DocHelpers
    {
        public static string GenerarQrAfip(DocFactura f)
        {
            var qrData = new
            {
                ver = 1,
                fecha = f.Fecha.ToString("yyyy-MM-dd"),
                cuit = f.Empresa?.Cuit,
                ptoVta = f.PuntoVenta,
                tipoCmp = f.CodigoAfip ?? 0,
                nroCmp = long.Parse(f.NroComprobante.Substring(8)),
                importe = f.Total,
                moneda = "PES",
                ctz = 1,
                tipoDocRec = f.AfipTipoDocumento ?? 99,
                nroDocRec = f.AfipNroDocumento ?? 0,
                tipoCodAut = f.AfipResultado,
                codAut = string.IsNullOrWhiteSpace(f.AfipCAE) ? 0 : long.Parse(f.AfipCAE)
            };

            var json = JsonSerializer.Serialize(qrData);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            return $"https://servicioscf.afip.gob.ar/publico/comprobantes/cae.aspx?p={base64}";
        }
    }
}
