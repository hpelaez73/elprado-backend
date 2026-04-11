namespace ElPrado.Dto.Documents
{
    public class DocFactura
    {
        public string NroComprobante { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public string TipoComprobante { get; set; } = string.Empty;
        public string Talonario { get; set; } = string.Empty;
        public string Letra { get; set; } = string.Empty;
        public int PuntoVenta { get; set; }
        public int? CodigoAfip { get; set; }

        public int? Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public string ZonaCobranza { get; set; } = string.Empty;
        public string Cobrador { get; set; } = string.Empty;

        public double Neto { get; set; }
        public double Iva { get; set; }
        public double NoGravado { get; set; }
        public double AjusteGravado { get; set; }
        public double Total { get; set; }
        public string ImporteEnLetras { get; set; } = string.Empty;

        public string TextoMonotributo { get; set; } = string.Empty;
        public string AfipResultado { get; set; } = string.Empty;
        public string AfipCAE { get; set; } = string.Empty;
        public DateTime? AfipVencimientoCAE { get; set; }
        public int? AfipTipoDocumento { get; set; }
        public long? AfipNroDocumento { get; set; }

        public string Observaciones { get; set; } = string.Empty;
        public bool Pagado { get; set; }

        public DocEmpresaFactura? Empresa { get; set; }
        public DocClienteFactura? Cliente { get; set; }
        public DocBeneficiarioFactura? Beneficiario { get; set; }
        public List<DocDetalleFactura>? Detalles { get; set; }
        public DocImagenesFactura? Imagenes { get; set; }
    }

    public class DocFacturasList
    {
        public List<DocFactura> Facturas { get; set; } = new List<DocFactura>();
    }

    public class DocEmpresaFactura
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Telefono24hs { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CategoriaIva { get; set; } = string.Empty;
        public long Cuit { get; set; }
        public string IngresosBrutos { get; set; } = string.Empty;
        public DateOnly InicioActividades { get; set; }
    }

    public class DocClienteFactura
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CategoriaIva { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long? NroDocumento { get; set; }
    }

    public class DocDetalleFactura
    {
        public double? Cantidad { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public double PrecioUnitario { get; set; }
        public double Total { get; set; }
        public double AlicuotaIva { get; set; }
    }

    public class DocBeneficiarioFactura
    {
        public string Nombre { get; set; } = string.Empty;
        public long? NroDocumento { get; set; }
        public DateOnly? FechaUsoBeneficio { get; set; }
    }

    public class DocImagenesFactura
    {
        public byte[]? LogoEmpresa { get; set; }
        public byte[]? LogoArca { get; set; }
        public byte[]? SelloPagado { get; set; }
    }
}
