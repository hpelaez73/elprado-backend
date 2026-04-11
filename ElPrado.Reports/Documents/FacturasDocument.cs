using ElPrado.Dto.Documents;
using ElPrado.Reports.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;

namespace ElPrado.Reports.Documents
{
    public class FacturasDocument : IDocument
    {
        private readonly DocFacturasList _data;

        public FacturasDocument(DocFacturasList data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            foreach (var factura in _data.Facturas)
            {
                container
                    .Page(page =>
                    {
                        //page.Size(PageSizes.A4);
                        page.Margin(20);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Element(c => ComposeHeader(c, factura));
                        page.Content().Layers(layers =>
                        {
                            // ===== CAPA DE WATERMARK =====
                            if (factura.Pagado && factura.Imagenes?.SelloPagado != null)
                            {
                                layers.Layer().AlignCenter().AlignMiddle().Element(container =>
                                {
                                    container
                                        .Width(250)
                                        .Image(factura.Imagenes.SelloPagado);
                                });
                            }

                            // ===== CONTENIDO NORMAL =====
                            layers.PrimaryLayer().Element(c => ComposeContent(c, factura));
                        });

                        page.Footer().Element(c => ComposeFooter(c, factura));
                    });
            }
        }

        private void ComposeHeader(IContainer container, DocFactura f)
        {
            container
                .Border(1)
                .Column(col =>
                {
                    col.Item().Element(c => ComposeHeaderFactura(c, f));
                    col.Item().Element(c => ComposeEmpresa(c, f));
                });
        }

        private void ComposeHeaderFactura(IContainer container, DocFactura f)
        {
            container
                .Row(row =>
                {
                    // ================= LEFT =================
                    row.RelativeItem(5).Column(col =>
                    {
                        if (f.Imagenes?.LogoEmpresa != null)
                        {
                            col.Item()
                                .AlignLeft()
                                .Padding(5)
                                .Width(100)
                                .Height(40)
                                .Image(f.Imagenes.LogoEmpresa);
                        }
                    });

                    // ================= CENTER (LETRA + CODIGO) =================
                    row.ConstantItem(80).AlignCenter().AlignTop().Element(c =>
                    {
                        c.Border(1)
                            .Padding(5)
                            .Column(col =>
                            {
                                col.Item().AlignCenter()
                                    .Text(f.Letra)
                                    .Bold()
                                    .FontSize(20);

                                col.Item().AlignCenter()
                                    .Text($"COD {f.CodigoAfip:00}")
                                    .FontSize(8);
                            });
                    });

                    // ================= RIGHT =================
                    row.RelativeItem(5).PaddingLeft(25).Column(col =>
                    {
                        col.Spacing(5);

                        col.Item()
                            .PaddingTop(5)
                            .Text(f.TipoComprobante)
                            .Bold()
                            .FontSize(12);

                        col.Item()
                            .Text(f.NroComprobante)
                            .Bold()
                            .FontSize(18);
                    });
                });
        }

        private void ComposeEmpresa(IContainer container, DocFactura f)
        {
            container
                .PaddingLeft(5)
                .PaddingRight(5)
                .PaddingBottom(5)
                .Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(f.Empresa?.Nombre)
                            .Bold().FontSize(12);

                        col.Item().Text(f.Empresa?.Direccion)
                            .FontSize(8);
                        col.Item().Text($"Tel: {f.Empresa?.Telefono}")
                            .FontSize(8);
                        col.Item().Text($"Atención las 24 hs: {f.Empresa?.Telefono24hs}")
                            .FontSize(8);

                        col.Item().Text("").FontSize(1);

                        col.Item().Text($"E-Mail: {f.Empresa?.Email}")
                            .FontSize(8);

                        col.Item().Text(f.Empresa?.CategoriaIva)
                            .Bold().FontSize(8);
                    });

                    row.AutoItem()
                        .PaddingHorizontal(15)
                        .LineVertical(1);

                    row.RelativeItem().PaddingLeft(50).Column(col =>
                    {
                        col.Item()
                            .Text($"Fecha: {f.Fecha:dd/MM/yyyy}")
                            .Bold()
                            .FontSize(12);

                        col.Item().Text("").FontSize(10);

                        col.Item()
                            .Text($"C.U.I.T.: {f.Empresa?.Cuit:00-00000000-0}")
                            .FontSize(8);
                        col.Item()
                            .Text($"Ingresos Brutos: {f.Empresa?.IngresosBrutos}")
                            .FontSize(8);
                        col.Item()
                            .Text($"Inicio de Actividades: {f.Empresa?.InicioActividades:dd/MM/yyyy}")
                            .FontSize(8);
                    });
                });

        }

        private void ComposeContent(IContainer container, DocFactura f)
        {
            container
                .Border(1)
                .Column(col =>
                {
                    col.Item().Element(c => ComposeCliente(c, f));
                    col.Item().LineHorizontal(1);
                    col.Item().Element(c => ComposePropuesta(c, f));
                    col.Item().LineHorizontal(1);
                    col.Item().Element(c => ComposeDetalle(c, f));
                    col.Item().Element(c => ComposeObservaciones(c, f));
                });
        }

        private void ComposeCliente(IContainer container, DocFactura f)
        {
            container
                .Padding(5)
                .Table(table =>
                {
                    // Definimos 4 columnas base
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80);   // Label 1
                        columns.RelativeColumn();     // Valor 1
                        columns.ConstantColumn(60);   // Label 2
                        columns.RelativeColumn();     // Valor 2
                    });

                    // ===== FILA 1 =====
                    table.Cell().PaddingVertical(1)
                        .Text("Titular")
                        .Bold();
                    table.Cell().ColumnSpan(3).PaddingVertical(1)
                        .Text(f.Cliente?.Nombre);

                    // ===== FILA 2 =====
                    table.Cell().PaddingVertical(1)
                        .Text("Domicilio")
                        .Bold();
                    table.Cell().ColumnSpan(3).PaddingVertical(1)
                        .Text(f.Cliente?.Direccion);

                    // ===== FILA 3 =====
                    table.Cell().PaddingVertical(1)
                        .Text("Localidad")
                        .Bold();
                    table.Cell().ColumnSpan(3).PaddingVertical(1)
                        .Text(f.Cliente?.Localidad);

                    // ===== FILA 4 (4 columnas reales) =====
                    table.Cell().PaddingVertical(1).Text("I.V.A.")
                        .Bold();
                    table.Cell().PaddingVertical(1)
                        .Text(f.Cliente?.CategoriaIva);

                    table.Cell().PaddingVertical(1)
                        .Text(f.Cliente?.TipoDocumento)
                        .Bold();
                    table.Cell().PaddingVertical(1)
                        .Text((f.AfipTipoDocumento == 80) ? f.Cliente?.NroDocumento?.ToString("00-00000000-0") : f.Cliente?.NroDocumento?.ToString());

                    // ===== FILA 5 =====
                    table.Cell().PaddingVertical(1)
                        .Text("Teléfono")
                        .Bold();
                    table.Cell().ColumnSpan(3).PaddingVertical(1)
                        .Text(f.Cliente?.Telefono);
                });
        }

        private void ComposePropuesta(IContainer container, DocFactura f)
        {
            container
                .Padding(2)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().PaddingVertical(1)
                        .Text("Propuesta")
                        .Bold()
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text("Parcela")
                        .Bold()
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text("Sector")
                        .Bold()
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text("Manzana")
                        .Bold()
                        .AlignCenter();

                    table.Cell().ColumnSpan(2).PaddingVertical(1)
                        .Text("Zona")
                        .Bold()
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text(f.Propuesta?.ToString())
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text(f.Parcela)
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text(f.Sector)
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text(f.Manzana)
                        .AlignCenter();

                    table.Cell().PaddingVertical(1)
                        .Text(f.ZonaCobranza)
                        .AlignCenter();
                    table.Cell().PaddingVertical(1)
                        .Text(f.Cobrador)
                        .AlignCenter();

                });
        }

        private void ComposeDetalle(IContainer container, DocFactura f)
        {
            var letraA = f.Letra.Equals("A", StringComparison.OrdinalIgnoreCase);

            container
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);
                    columns.RelativeColumn();
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(80);
                    columns.ConstantColumn(80);
                });

                // HEADER
                table.Header(header =>
                {
                    header
                        .Cell().BorderBottom(1).AlignCenter().Padding(5)
                        .Text("Cant").Bold();
                    header
                        .Cell().BorderBottom(1).PaddingVertical(5)
                        .Text("Producto / Servicio").Bold();
                    header
                        .Cell().BorderBottom(1).AlignRight().PaddingVertical(5)
                        .Text((letraA) ? "% IVA" : "").Bold();
                    header
                        .Cell().BorderBottom(1).AlignRight().PaddingVertical(5)
                        .Text("Precio Unit").Bold();
                    header
                        .Cell().BorderBottom(1).AlignRight().Padding(5)
                        .Text((letraA) ? "Subtotal c/IVA" : "Subtotal").Bold();
                });

                if (f.Detalles == null || f.Detalles.Count == 0)
                {
                    table.Cell().ColumnSpan(5).AlignCenter().Text("No hay detalles");
                    return;
                }

                foreach (var item in f.Detalles)
                {
                    table.Cell().AlignCenter()
                        .Text(item.Cantidad.ToString());

                    table.Cell().Text(item.Descripcion);

                    table.Cell().AlignRight()
                        .Text((letraA) ? item.AlicuotaIva.ToString("N2") : "");

                    table.Cell().AlignRight()
                        .Text(item.PrecioUnitario.ToString("N2"));

                    table.Cell().AlignRight().PaddingRight(5)
                        .Text(item.Total.ToString("N2"));
                }
            });
        }

        private void ComposeObservaciones(IContainer container, DocFactura f)
        {
            container
                .Padding(5)
                .Text(f.Observaciones);
        }

        private void ComposeFooter(IContainer container, DocFactura f)
        {
            var letraA = f.Letra.Equals("A", StringComparison.OrdinalIgnoreCase);

            container
                .Border(1)
                .Column(col =>
                {
                    col.Item().ShowIf(letraA).Element(c => ComposeTotalesA(c, f));

                    col.Item().ShowIf(!letraA).Element(c => ComposeTotalesB(c, f));
                    col.Item().ShowIf(!letraA).LineHorizontal(1);
                    col.Item().ShowIf(!letraA).Element(c => ComposeTotalesBExtra(c, f));

                    col.Item().LineHorizontal(1);

                    col.Item().Element(c => ComposeAfip(c, f));
                });
        }

        private void ComposeTotalesA(IContainer container, DocFactura f)
        {
            bool tieneBeneficiario = f.Beneficiario != null && !string.IsNullOrEmpty(f.Beneficiario.Nombre);

            container
                .Padding(5)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(70);
                        columns.RelativeColumn();
                        columns.ConstantColumn(80);
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                        columns.ConstantColumn(80);
                    });

                    // Fila 1
                    table.Cell().ColumnSpan(4).RowSpan(2)
                        .Text($"Son pesos {f.ImporteEnLetras} ***");

                    table.Cell().AlignRight()
                        .Text("Neto No Gravado:").Bold();
                    table.Cell().AlignRight()
                        .Text(f.NoGravado.ToString("N2"));

                    // Fila 2
                    table.Cell().AlignRight()
                        .Text("Neto Gravado:").Bold();
                    table.Cell().AlignRight()
                        .Text(f.Neto.ToString("N2"));

                    // Fila 3
                    if (tieneBeneficiario)
                    {
                        table.Cell()
                            .Text("Beneficiario:").Bold();
                        table.Cell().ColumnSpan(3)
                            .Text(f.Beneficiario?.Nombre);
                    } 
                    else
                    {
                        table.Cell().ColumnSpan(4)
                            .Text("");
                    }

                    table.Cell().AlignRight()
                        .Text("Ajuste por Redondeo:").Bold();
                    table.Cell().AlignRight()
                        .Text(f.AjusteGravado.ToString("N2"));

                    // Fila 4
                    if (tieneBeneficiario)
                    {
                        table.Cell()
                            .Text("Documento:").Bold();
                        table.Cell()
                            .Text(f.Beneficiario?.NroDocumento.ToString());

                        table.Cell()
                            .Text("Uso del servicio:").Bold();
                        table.Cell()
                            .Text(f.Beneficiario?.FechaUsoBeneficio?.ToString("dd/MM/yyyy"));
                    }
                    else
                    { 
                        table.Cell().ColumnSpan(4).Text(""); 
                    }

                    table.Cell().AlignRight()
                        .Text("IVA:").Bold();
                    table.Cell().AlignRight()
                        .Text(f.Iva.ToString("N2"));

                    // Fila 5
                    table.Cell().ColumnSpan(4)
                        .Text(f.TextoMonotributo)
                        .FontSize(8);

                    table.Cell().AlignRight().AlignBottom()
                        .Text("Total:").Bold();
                    table.Cell().AlignRight().AlignBottom()
                        .Text(f.Total.ToString("N2")).Bold();
                });

        }

        private void ComposeTotalesB(IContainer container, DocFactura f)
        {
            bool tieneBeneficiario = f.Beneficiario != null && !string.IsNullOrEmpty(f.Beneficiario.Nombre);

            container
                .Padding(5)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(70);
                        columns.RelativeColumn();
                        columns.ConstantColumn(80);
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                        columns.ConstantColumn(80);
                    });

                    // Fila 1
                    table.Cell().ColumnSpan(4).RowSpan(2)
                        .Text($"Son pesos {f.ImporteEnLetras} ***");
                    table.Cell().AlignRight()
                        .Text("Importe Total:")
                        .Bold();
                    table.Cell().AlignRight()
                        .Text(f.Total.ToString("N2"));

                    // Fila 2
                    table.Cell().ColumnSpan(2)
                        .Text("");

                    // Fila 3
                    if (tieneBeneficiario)
                    {
                        table.Cell()
                        .Text("Beneficiario:").Bold();
                        table.Cell().ColumnSpan(5)
                            .Text(f.Beneficiario?.Nombre);
                    }
                    else
                    {
                        table.Cell().ColumnSpan(6)
                            .Text("");
                    }

                    // Fila 4
                    if (tieneBeneficiario)
                    {
                        table.Cell()
                            .Text("Documento:").Bold();
                        table.Cell()
                            .Text(f.Beneficiario?.NroDocumento.ToString());
                        table.Cell()
                            .Text("Uso del servicio:").Bold();
                        table.Cell()
                            .Text(f.Beneficiario?.FechaUsoBeneficio?.ToString("dd/MM/yyyy"));
                    }
                });

        }

        private void ComposeTotalesBExtra(IContainer container, DocFactura f)
        {
            container
                .Padding(5)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(205);
                        columns.ConstantColumn(60);
                        columns.RelativeColumn();
                    });

                    table.Cell().ColumnSpan(3)
                        .Text("Régimen de Transparencia Fiscal al Consumidor (Ley 27.743)")
                        .Bold()
                        .Underline();

                    table.Cell().AlignRight()
                        .Text("IVA Contenido:");
                    table.Cell().AlignRight()
                        .Text(f.Iva.ToString("N2"));
                    table.Cell().Text("");

                    table.Cell().AlignRight()
                        .Text("Otros Impuestos Nacionales Indirectos:");
                    table.Cell().AlignRight()
                        .Text(("0.00"));
                    table.Cell().Text("");
                });
        }

        private void ComposeAfip(IContainer container, DocFactura f)
        {
            container
                .Padding(5)
                .Column(col =>
                {
                    col.Spacing(2);
                    col.Item().Row(row =>
                    {
                        // ===== 1. QR =====
                        row.ConstantItem(70).Column(c =>
                        {
                            c.Item()
                                .AspectRatio(1)
                                .Background(Colors.White)
                                .Svg(size =>
                                {
                                    var writer = new QRCodeWriter();
                                    var qrCode = writer.encode(DocHelpers.GenerarQrAfip(f), BarcodeFormat.QR_CODE, (int)size.Width, (int)size.Height);
                                    var renderer = new SvgRenderer { FontName = "Lato" };
                                    return renderer.Render(qrCode, BarcodeFormat.EAN_13, null).Content;
                                });
                        });

                        // ===== 2. LOGO AFIP + TEXTO =====
                        row.ConstantItem(140).Column(c =>
                        {
                            if (f.Imagenes?.LogoArca != null)
                            {
                                c.Item()
                                .AlignCenter()
                                .Width(100)
                                .Height(50)
                                .Image(f.Imagenes.LogoArca); // 🔁 tu logo AFIP
                            }

                            c.Item().AlignCenter()
                                .Text("Comprobante autorizado")
                                .FontSize(8)
                                .Bold();
                        });

                        row.RelativeItem()
                            .Text("");

                        // ===== 3. CAE =====
                        row.ConstantItem(170).Column(c =>
                        {
                            c.Spacing(2);

                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Text("C.A.E. Nº:").Bold();
                                r.RelativeItem().AlignRight()
                                    .Text(f.AfipCAE);
                            });

                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Vencimiento").Bold();
                                r.RelativeItem().AlignRight()
                                    .Text(f.AfipVencimientoCAE?.ToString("dd/MM/yyyy"));
                            });
                        });
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignCenter()
                            .Text("ORIGINAL")
                            .Bold();
                    });
                });

        }

    }
}
