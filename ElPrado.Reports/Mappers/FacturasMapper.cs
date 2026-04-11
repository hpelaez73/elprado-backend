using ElPrado.Core;
using ElPrado.Core.Domains;
using ElPrado.Core.Utils;
using ElPrado.Dto.Documents;
using ElPrado.Dto.Dtos;

namespace ElPrado.Reports.Mappers
{
    public static class FacturasMapper
    {
        public static DocFactura MapToDoc(DtoComprobantes comprobante)
        {
            DocFactura factura = new()
            {
                NroComprobante = comprobante.NroComprobante,
                Fecha = comprobante.Fecha,
                TipoComprobante = comprobante.TipoComprobante,
                Talonario = comprobante.Talonario,
                Letra = comprobante.Letra,
                PuntoVenta = comprobante.PuntoVenta,
                CodigoAfip = comprobante.CodigoAfip,
                AfipTipoDocumento = comprobante.AfipTipoDocumento,
                AfipNroDocumento = comprobante.AfipNroDocumento,

                Propuesta = comprobante.Propuesta,
                Parcela = comprobante.Parcela,
                Sector = comprobante.Sector,
                Manzana = comprobante.Manzana,
                ZonaCobranza = comprobante.ZonaCobranza,
                Cobrador = comprobante.Cobrador,

                Neto = comprobante.Neto,
                Iva = comprobante.Iva,
                NoGravado = comprobante.NoGravado,
                AjusteGravado = comprobante.Redondeo,
                Total = comprobante.Total,
                ImporteEnLetras = FunUtils.ImporteEnLetras(comprobante.Total),

                TextoMonotributo = comprobante.Letra.Equals("A", StringComparison.OrdinalIgnoreCase) && comprobante.CategoriaIva == CategoriaIvaDomain.ResponsableMonotributo
                    ? ConstAfip.TextoMonotributo
                    : string.Empty,
                AfipResultado = comprobante.AfipResultado,
                AfipCAE = comprobante.AfipCAE,
                AfipVencimientoCAE = comprobante.AfipVencimientoCAE,

                Observaciones = comprobante.Observaciones,
                Pagado = comprobante.Pagado,

                Empresa = new DocEmpresaFactura
                {
                    Nombre = "Origen S.A.",
                    Direccion = "Parque: Ruta 33 Km 847.5 - 2121 Pérez - Santa Fe",
                    Telefono = "0341 - 495 1000",
                    Telefono24hs = "0341 - 156 163758",
                    Email = "elprado@origensa.com.ar",
                    CategoriaIva = "I.V.A. RESPONSABLE INSCRIPTO",
                    Cuit = 30614762221,
                    IngresosBrutos = "021-192619-1",
                    InicioActividades = new DateOnly(1988, 4, 15)
                },

                Cliente = new DocClienteFactura
                {
                    Nombre = comprobante.Cliente,
                    Direccion = comprobante.Direccion,
                    Localidad = comprobante.Localidad,
                    Telefono = comprobante.Telefono,
                    CategoriaIva = comprobante.CategoriaIva,
                    TipoDocumento = (comprobante.AfipTipoDocumento == 80) ? "C.U.I.T." : comprobante.TipoDocumento,
                    NroDocumento = (comprobante.AfipTipoDocumento == 80) ? comprobante.AfipNroDocumento : comprobante.NroDocumento
                },

                Beneficiario = new DocBeneficiarioFactura
                {
                    Nombre = comprobante.ClienteBeneficiado,
                    NroDocumento = comprobante.DocumentoBeneficiado,
                    FechaUsoBeneficio = comprobante.FechaUtilizacion
                },

                Detalles = comprobante.ListDetalles?.Select(d => new DocDetalleFactura
                {
                    Descripcion = d.Detalle,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Total = d.Total,
                    AlicuotaIva = d.Porcentaje
                }).ToList()
            };
            return factura;
        }
    }
}
