using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using NSubstitute;
using System.Text.Json;
using Xunit;

namespace ElPrado.Tests;

public class EstadisticasServiceTests
{
    [Theory]
    [InlineData("2026-2")]
    [InlineData("2026-13")]
    [InlineData("texto")]
    public void Resumen_RechazaPeriodoInvalido(string periodo)
    {
        EstadisticasService servicio = new(Substitute.For<IUnitOfWork>(), Substitute.For<IUserContextService>());

        var resultado = servicio.Resumen(periodo, "todos");

        Assert.True(resultado.HayError);
    }

    [Fact]
    public void Evolucion_RechazaProductoFueraDelContrato()
    {
        EstadisticasService servicio = new(Substitute.For<IUnitOfWork>(), Substitute.For<IUserContextService>());

        var resultado = servicio.Evolucion("2026-01", "2026-02", "otro");

        Assert.True(resultado.HayError);
    }

    [Fact]
    public void ResumenSinDatos_SerializaNulosYNoPublicaKpi()
    {
        DtoEstadisticasResumen dto = new()
        {
            DatosDisponibles = false,
            Periodo = "2026-01",
            Producto = "todos"
        };

        string json = JsonSerializer.Serialize(dto);

        Assert.Contains("\"DatosDisponibles\":false", json);
        Assert.Contains("\"Kpi\":null", json);
    }

    [Theory]
    [InlineData("todos", "todos", 4)]
    [InlineData("DPC", "dpc", 2)]
    [InlineData("flores-siempre", "flores-siempre", 1)]
    public void ResumenDisponible_NormalizaProductoCalculaTasasYExponeDeudoresRecientes(string productoSolicitado, string productoEsperado, int deudoresRecientes)
    {
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IEstadisticasRepository estadisticas = Substitute.For<IEstadisticasRepository>();
        unitOfWork.Estadisticas.Returns(estadisticas);
        estadisticas.Resumen(Arg.Any<DateTime>(), productoEsperado).Returns(new DtoEstadisticasFila
        {
            Periodo = new DateTime(2026, 9, 1),
            FechaCorte = new DateTime(2026, 9, 30),
            ActualizadoEn = new DateTime(2026, 9, 30, 23, 0, 0),
            CantidadParcelas = 4,
            AlDia = 3,
            Morosas = 1,
            Recuperadas = 1,
            DeudoresRecientes = deudoresRecientes
        });
        EstadisticasService servicio = new(unitOfWork, Substitute.For<IUserContextService>());

        var resultado = servicio.Resumen("2026-09", productoSolicitado);

        Assert.False(resultado.HayError);
        Assert.True(resultado.Valor!.DatosDisponibles);
        Assert.Equal(productoEsperado, resultado.Valor.Producto);
        Assert.Equal(75m, resultado.Valor.Kpi!.PorcentajeAlDia);
        Assert.Equal(25m, resultado.Valor.Kpi.PorcentajeMorosas);
        Assert.Equal(deudoresRecientes, resultado.Valor.Kpi.DeudoresRecientes);
        estadisticas.Received(1).Resumen(Arg.Any<DateTime>(), productoEsperado);
    }

    [Theory]
    [InlineData("todos", 4)]
    [InlineData("dpc", 2)]
    [InlineData("flores-siempre", 1)]
    public void Evolucion_EnumeraSoloPeriodosSinCoberturaSinInventarCerosYExponeDeudoresRecientes(string producto, int deudoresRecientes)
    {
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IEstadisticasRepository estadisticas = Substitute.For<IEstadisticasRepository>();
        unitOfWork.Estadisticas.Returns(estadisticas);
        estadisticas.Evolucion(Arg.Any<DateTime>(), Arg.Any<DateTime>(), producto).Returns(new List<DtoEstadisticasFila>
        {
            new() { Periodo = new DateTime(2026, 1, 1), FechaCorte = new DateTime(2026, 1, 31), ActualizadoEn = new DateTime(2026, 2, 1), CantidadParcelas = 0, DeudoresRecientes = deudoresRecientes }
        });
        estadisticas.Cobertura(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(new List<DtoEstadisticasCobertura>
        {
            new() { Periodo = new DateTime(2026, 1, 1) }
        });
        EstadisticasService servicio = new(unitOfWork, Substitute.For<IUserContextService>());

        var resultado = servicio.Evolucion("2026-01", "2026-03", producto);

        Assert.False(resultado.HayError);
        Assert.Single(resultado.Valor!.Observaciones);
        Assert.Equal(new[] { "2026-02", "2026-03" }, resultado.Valor.PeriodosFaltantes);
        Assert.Null(resultado.Valor.Observaciones[0].Kpi.PorcentajeAlDia);
        Assert.Equal(deudoresRecientes, resultado.Valor.Observaciones[0].Kpi.DeudoresRecientes);
        estadisticas.Received(1).Evolucion(Arg.Any<DateTime>(), Arg.Any<DateTime>(), producto);
    }
}
