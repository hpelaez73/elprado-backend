using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Interfaces;
using ElPrado.Services.Services;
using NSubstitute;
using System.Text.Json;
using Xunit;

namespace ElPrado.Tests;

public class ComprobantesServiceTests
{
    [Fact]
    public void DetalleAfip_SerializaCamposDescriptivosSinQuitarLosTecnicos()
    {
        string json = JsonSerializer.Serialize(new DtoAfipWsfeConsultaDetalle
        {
            Concepto = 1,
            ConceptoDescripcion = "Productos",
            DocTipo = 80,
            DocTipoDescripcion = "CUIT",
            CondicionIva = 5,
            CondicionIvaDescripcion = "Consumidor final"
        });

        Assert.Contains("\"Concepto\":1", json);
        Assert.Contains("\"ConceptoDescripcion\":\"Productos\"", json);
        Assert.Contains("\"DocTipo\":80", json);
        Assert.Contains("\"DocTipoDescripcion\":\"CUIT\"", json);
        Assert.Contains("\"CondicionIva\":5", json);
    }

    [Fact]
    public async Task ConsultarComprobanteAfipAsync_EnriqueceEquivalenciasYPriorizaCuit()
    {
        IUnitOfWork uow = Substitute.For<IUnitOfWork>();
        IComprobantesAfipRepository repositorio = Substitute.For<IComprobantesAfipRepository>();
        IAfipWsfeGateway gateway = Substitute.For<IAfipWsfeGateway>();
        uow.ComprobantesAfip.Returns(repositorio);
        gateway.ConsultarComprobanteAsync(4, "0001-00000001").Returns(new ApiResponse<DtoAfipWsfeConsultaDetalle>
        {
            Data = new DtoAfipWsfeConsultaDetalle
            {
                DocTipo = 96,
                DocNro = 123,
                CbteTipo = 6,
                Iva = new List<DtoAfipWsfeIva> { new() { Id = 5 } }
            }
        });
        repositorio.ObtenerEnriquecimientoAfipAsync(4, "0001-00000001", Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>())
            .Returns(new AfipWsfeConsultaEnriquecimiento
            {
                Cuit = 20123456789,
                TipoComprobante = 6,
                TipoComprobanteDescripcion = "Factura B",
                CondicionIva = 5,
                CondicionIvaDescripcion = "Consumidor final",
                TiposDocumentos = new Dictionary<int, string> { [80] = "CUIT" },
                TiposIva = new Dictionary<int, string> { [5] = "21%" }
            });

        ComprobantesService servicio = new(uow, Substitute.For<IUserContextService>());
        ApiResponse<DtoAfipWsfeConsultaDetalle> respuesta = await servicio.ConsultarComprobanteAfipAsync(4, "0001-00000001", gateway);

        Assert.True(respuesta.Success);
        Assert.Equal(80, respuesta.Data!.DocTipo);
        Assert.Equal(20123456789, respuesta.Data.DocNro);
        Assert.Equal("CUIT", respuesta.Data.DocTipoDescripcion);
        Assert.Equal("Factura B", respuesta.Data.CbteTipoDescripcion);
        Assert.Equal("21%", respuesta.Data.Iva[0].IdDescripcion);
    }

    [Fact]
    public async Task ConsultarComprobanteAfipAsync_ConservaCodigoSinEquivalencia()
    {
        IUnitOfWork uow = Substitute.For<IUnitOfWork>();
        IComprobantesAfipRepository repositorio = Substitute.For<IComprobantesAfipRepository>();
        IAfipWsfeGateway gateway = Substitute.For<IAfipWsfeGateway>();
        uow.ComprobantesAfip.Returns(repositorio);
        gateway.ConsultarComprobanteAsync(4, "0001-00000002").Returns(new ApiResponse<DtoAfipWsfeConsultaDetalle>
        {
            Data = new DtoAfipWsfeConsultaDetalle { DocTipo = 96, DocNro = 123 }
        });
        repositorio.ObtenerEnriquecimientoAfipAsync(4, "0001-00000002", Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>())
            .Returns(new AfipWsfeConsultaEnriquecimiento
            {
                NroDocumento = 123,
                TiposDocumentos = new Dictionary<int, string>()
            });

        ComprobantesService servicio = new(uow, Substitute.For<IUserContextService>());
        ApiResponse<DtoAfipWsfeConsultaDetalle> respuesta = await servicio.ConsultarComprobanteAfipAsync(4, "0001-00000002", gateway);

        Assert.Equal(96, respuesta.Data!.DocTipo);
        Assert.Equal(123, respuesta.Data.DocNro);
        Assert.Equal(string.Empty, respuesta.Data.DocTipoDescripcion);
    }

    [Fact]
    public async Task ConsultarComprobanteAfipAsync_ConservaErrorControladoDelGateway()
    {
        IUnitOfWork uow = Substitute.For<IUnitOfWork>();
        IComprobantesAfipRepository repositorio = Substitute.For<IComprobantesAfipRepository>();
        IAfipWsfeGateway gateway = Substitute.For<IAfipWsfeGateway>();
        uow.ComprobantesAfip.Returns(repositorio);
        gateway.ConsultarComprobanteAsync(4, "0001-00000003").Returns(new ApiResponse<DtoAfipWsfeConsultaDetalle>());
        gateway.ConsultarComprobanteAsync(4, "0001-00000003").Returns(new ApiResponse<DtoAfipWsfeConsultaDetalle>
        {
            Success = false,
            Errors = new List<string> { "No se pudo conectar con Afip.WebApi." }
        });

        ComprobantesService servicio = new(uow, Substitute.For<IUserContextService>());
        ApiResponse<DtoAfipWsfeConsultaDetalle> respuesta = await servicio.ConsultarComprobanteAfipAsync(4, "0001-00000003", gateway);

        Assert.False(respuesta.Success);
        Assert.Null(respuesta.Data);
        await repositorio.DidNotReceive().ObtenerEnriquecimientoAfipAsync(
            Arg.Any<int>(), Arg.Any<string>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>(), Arg.Any<IEnumerable<int>>());
    }
}
