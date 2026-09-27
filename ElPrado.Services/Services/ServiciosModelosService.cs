using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ServiciosModelosService : ServiceBase
    {
        public ServiciosModelosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public DtoServiciosPropuesta ServiciosPropuesta(int codPropuesta)
        {
            DtoServiciosPropuesta serviciosPropuesta = new()
            {
                ListServiciosHabilitados = _uow.ServiciosModelos.ServiciosPropuesta(codPropuesta),
                ListServiciosUtilizados = _uow.ServiciosModelos.ServiciosUtilizadosPropuesta(codPropuesta),
                ListServiciosBeneficiarios = _uow.ServiciosModelos.BeneficiariosPropuesta(codPropuesta)
            };
            return serviciosPropuesta;
        }

        public DtoServiciosPropuestaMcp ServiciosPropuestaMcp(int codPropuesta)
        {
            List<DtoServicioPropuestaMcpFuente> fuente = _uow.ServiciosModelos.ServiciosPropuestaMcp(codPropuesta);
            return new DtoServiciosPropuestaMcp
            {
                Habilitaciones = fuente.Select(MapHabilitacion).ToList(),
                Cupos = fuente
                    .Where(x => x.LimiteServicios > 0)
                    .GroupBy(x => new { x.CodServicioModelo, x.Orden, x.CodPlanVenta, x.Modelo, x.Servicio })
                    .Select(x => x.First())
                    .Select(x => new DtoCupoServicioMcp
                    {
                        Producto = x.Modelo,
                        Servicio = x.Servicio,
                        Total = x.LimiteServicios,
                        Utilizados = x.ServiciosRealizados,
                        Disponibles = x.ServiciosPendientes
                    })
                    .ToList(),
                Utilizaciones = _uow.ServiciosModelos.UtilizacionesPropuestaMcp(codPropuesta)
            };
        }

        private static DtoHabilitacionServicioMcp MapHabilitacion(DtoServicioPropuestaMcpFuente source)
        {
            (string? codigo, string? descripcion, DateOnly? hasta) = ObtenerMotivo(source);
            return new DtoHabilitacionServicioMcp
            {
                CodCliente = source.CodCliente,
                Cliente = source.Cliente,
                Producto = source.Modelo,
                Servicio = source.Servicio,
                Habilitado = source.Habilitado,
                MotivoCodigo = codigo,
                MotivoDescripcion = descripcion,
                MotivoHasta = hasta
            };
        }

        private static (string? Codigo, string? Descripcion, DateOnly? Hasta) ObtenerMotivo(DtoServicioPropuestaMcpFuente source)
        {
            if (source.Habilitado) return (null, null, null);
            if (source.UtilizadoTitular > 0 || (source.LimiteServicios > 0 && source.ServiciosPendientes == 0))
                return ("MAXIMO_UTILIZADO", "Sin servicio: máximo utilizado.", null);
            if (source.MayorEdad) return ("EDAD", "Sin servicio por edad.", null);
            if (source.FechaCaducidad is { } caducidad && caducidad < DateOnly.FromDateTime(DateTime.Today))
                return ("PRODUCTO_CADUCADO", "Sin servicio: el producto ha caducado.", null);
            if (source.VigenciaDesde is { } desde && DateOnly.FromDateTime(DateTime.Today) < desde)
                return ("CARENCIA", "Sin servicio durante el período de carencia.", desde);
            if (source.VigenciaHasta is { } hasta && DateOnly.FromDateTime(DateTime.Today) > hasta)
                return ("NO_HABILITADO", "Sin servicio.", null);
            if (source.InhabilitadoPorMora && source.EnMora)
                return ("MORA", "Sin servicio por mora.", null);
            if (source.SuspendidoHasta is { } suspendido && suspendido > DateOnly.FromDateTime(DateTime.Today))
                return ("CARENCIA", "Sin servicio durante la suspensión.", suspendido);
            return ("NO_HABILITADO", "Sin servicio.", null);
        }
    }
}
