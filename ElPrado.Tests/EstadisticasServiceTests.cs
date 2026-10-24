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

    [Fact]
    public void ResumenDisponible_NormalizaProductoYCalculaTasas()
    {
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Estadisticas.Resumen(Arg.Any<DateTime>(), "dpc").Returns(new DtoEstadisticasFila
        {
            Periodo = new DateTime(2026, 9, 1),
            FechaCorte = new DateTime(2026, 9, 30),
            ActualizadoEn = new DateTime(2026, 9, 30, 23, 0, 0),
            CantidadParcelas = 4,
            AlDia = 3,
            Morosas = 1,
            Recuperadas = 1
        });
        EstadisticasService servicio = new(unitOfWork, Substitute.For<IUserContextService>());

        var resultado = servicio.Resumen("2026-09", "DPC");

        Assert.False(resultado.HayError);
        Assert.True(resultado.Valor!.DatosDisponibles);
        Assert.Equal("dpc", resultado.Valor.Producto);
        Assert.Equal(75m, resultado.Valor.Kpi!.PorcentajeAlDia);
        Assert.Equal(25m, resultado.Valor.Kpi.PorcentajeMorosas);
    }

    [Fact]
    public void Evolucion_EnumeraSoloPeriodosSinCoberturaYSinInventarCeros()
    {
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Estadisticas.Evolucion(Arg.Any<DateTime>(), Arg.Any<DateTime>(), "todos").Returns(new List<DtoEstadisticasFila>
        {
            new() { Periodo = new DateTime(2026, 1, 1), FechaCorte = new DateTime(2026, 1, 31), ActualizadoEn = new DateTime(2026, 2, 1), CantidadParcelas = 0 }
        });
        unitOfWork.Estadisticas.Cobertura(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(new List<DtoEstadisticasCobertura>
        {
            new() { Periodo = new DateTime(2026, 1, 1) }
        });
        EstadisticasService servicio = new(unitOfWork, Substitute.For<IUserContextService>());

        var resultado = servicio.Evolucion("2026-01", "2026-03", null);

        Assert.False(resultado.HayError);
        Assert.Single(resultado.Valor!.Observaciones);
        Assert.Equal(new[] { "2026-02", "2026-03" }, resultado.Valor.PeriodosFaltantes);
        Assert.Null(resultado.Valor.Observaciones[0].Kpi.PorcentajeAlDia);
    }
}
