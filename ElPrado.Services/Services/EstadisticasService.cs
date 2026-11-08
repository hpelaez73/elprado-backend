using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
using System.Globalization;

namespace ElPrado.Services.Services
{
    public sealed class EstadisticasService : ServiceBase
    {
        private static readonly HashSet<string> Productos = new(StringComparer.OrdinalIgnoreCase)
        {
            "todos", "dpc", "flores-siempre"
        };

        public EstadisticasService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext) { }

        public Resultados<DtoEstadisticasResumen> Resumen(string periodoTexto, string? productoTexto)
        {
            Resultados<DtoEstadisticasResumen> resultado = new();
            if (!TryValidar(periodoTexto, productoTexto, out DateTime periodo, out string producto, out string error))
            {
                resultado.Agregar(error);
                return resultado;
            }

            DtoEstadisticasFila? fila = _uow.Estadisticas.Resumen(periodo, producto);
            resultado.Valor = fila is null
                ? new DtoEstadisticasResumen { DatosDisponibles = false, Periodo = periodo.ToString("yyyy-MM"), Producto = producto }
                : new DtoEstadisticasResumen
                {
                    DatosDisponibles = true,
                    Periodo = fila.Periodo.ToString("yyyy-MM"),
                    PeriodoCorte = fila.FechaCorte,
                    ActualizadoEn = fila.ActualizadoEn,
                    Producto = producto,
                    Kpi = CrearKpi(fila)
                };
            return resultado;
        }

        public Resultados<DtoEstadisticasEvolucion> Evolucion(string desdeTexto, string hastaTexto, string? productoTexto)
        {
            Resultados<DtoEstadisticasEvolucion> resultado = new();
            if (!TryValidar(desdeTexto, productoTexto, out DateTime desde, out string producto, out string error) ||
                !TryPeriodo(hastaTexto, out DateTime hasta))
            {
                resultado.Agregar(string.IsNullOrEmpty(error) ? "El período hasta debe tener formato YYYY-MM." : error);
                return resultado;
            }
            if (desde > hasta)
            {
                resultado.Agregar("El período desde no puede ser posterior a hasta.");
                return resultado;
            }

            List<DtoEstadisticasFila> filas = _uow.Estadisticas.Evolucion(desde, hasta, producto);
            HashSet<DateTime> disponibles = _uow.Estadisticas.Cobertura(desde, hasta).Select(x => x.Periodo.Date).ToHashSet();
            DtoEstadisticasEvolucion evolucion = new()
            {
                Desde = desde.ToString("yyyy-MM"), Hasta = hasta.ToString("yyyy-MM"), Producto = producto,
                Observaciones = filas.Select(x => new DtoEstadisticasObservacion { Periodo = x.Periodo.ToString("yyyy-MM"), FechaCorte = x.FechaCorte, ActualizadoEn = x.ActualizadoEn, Kpi = CrearKpi(x) }).ToList()
            };
            for (DateTime mes = desde; mes <= hasta; mes = mes.AddMonths(1))
                if (!disponibles.Contains(mes.Date)) evolucion.PeriodosFaltantes.Add(mes.ToString("yyyy-MM"));
            resultado.Valor = evolucion;
            return resultado;
        }

        private static bool TryValidar(string periodoTexto, string? productoTexto, out DateTime periodo, out string producto, out string error)
        {
            producto = string.IsNullOrWhiteSpace(productoTexto) ? "todos" : productoTexto.Trim().ToLowerInvariant();
            if (!TryPeriodo(periodoTexto, out periodo)) { error = "El período debe tener formato YYYY-MM."; return false; }
            if (!Productos.Contains(producto)) { error = "El producto debe ser todos, dpc o flores-siempre."; return false; }
            error = string.Empty;
            return true;
        }

        private static bool TryPeriodo(string texto, out DateTime periodo) =>
            DateTime.TryParseExact(texto, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out periodo);

        private static DtoKpiParcelas CrearKpi(DtoEstadisticasFila fila) => new()
        {
            Parcelas = fila.CantidadParcelas, AlDia = fila.AlDia, Morosas = fila.Morosas, Recuperadas = fila.Recuperadas,
            DeudoresRecientes = fila.DeudoresRecientes
        };
    }
}
