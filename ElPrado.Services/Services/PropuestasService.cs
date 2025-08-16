using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class PropuestasService : ServiceBase
    {
        public PropuestasService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public Resultados<List<DtoClientesPropuestas>> Titulares(DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<List<DtoClientesPropuestas>> resultado = new();

            Resultados<Propuestas> resPropuesta = BuscarPropuesta(dtoPropuesta);
            if (resPropuesta.HayError || resPropuesta.Valor is null)
            {
                resultado.Agregar(resPropuesta);
                return resultado;
            }
            Propuestas propuesta = resPropuesta.Valor;

            resultado.Valor = _uow.Clientes.BuscarTitulares(propuesta.CodPropuesta);

            return resultado;
        }

        public Resultados<DtoPropuestaDetalleResp> Detalle(DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<DtoPropuestaDetalleResp> resultado = new();

            Resultados<Propuestas> resPropuesta = BuscarPropuesta(dtoPropuesta);
            if (resPropuesta.HayError || resPropuesta.Valor is null)
            {
                resultado.Agregar(resPropuesta);
                return resultado;
            }
            Propuestas propuesta = resPropuesta.Valor;
            DtoPropuestaDetalleResp? propuestaDetalle = _uow.Propuestas.BuscarPropuestaDetalle(propuesta.CodPropuesta);

            if (propuestaDetalle != null)
            {
                propuestaDetalle.ListPropuestasAsociadas = _uow.Propuestas.BuscarPropuestasAsociadas(propuestaDetalle.CodPropuesta, dtoPropuesta.IncluirBaja);

                if (propuestaDetalle.CodParcela != null)
                {
                    propuestaDetalle.ListZonasParcelas = _uow.Parcelas.BuscarZonasParcelas(propuestaDetalle.CodParcela.Value);
                    propuestaDetalle.ListDetalleLugares = _uow.Parcelas.BuscarDetalleLugares(propuestaDetalle.CodParcela.Value, propuestaDetalle.CodPropuesta);
                    propuestaDetalle.ListInhumados = _uow.Inhumados.BuscarInhumados(propuestaDetalle.CodPropuesta, propuestaDetalle.CodParcela.Value);
                }
            }

            resultado.Valor = propuestaDetalle;

            return resultado;
        }

        public Resultados<DtoPropuestaDetalleContratosResp> DetalleContratos(DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<DtoPropuestaDetalleContratosResp> resultado = new();

            Resultados<Propuestas> resPropuesta = BuscarPropuesta(dtoPropuesta);
            if (resPropuesta.HayError || resPropuesta.Valor is null)
            {
                resultado.Agregar(resPropuesta);
                return resultado;
            }
            Propuestas propuesta = resPropuesta.Valor;

            DtoPropuestaDetalleContratosResp propuestaDetalle = new()
            {
                ListContratos = _uow.Contratos.BuscarContratos(propuesta.CodPropuesta, dtoPropuesta.IncluirBaja),
                ListPlanesVentas = _uow.PlanesVentas.BuscarPlanesVentas(propuesta.CodPropuesta, dtoPropuesta.IncluirBaja),
                ListTitulares = _uow.Clientes.BuscarTitulares(propuesta.CodPropuesta),
                ListTitularesFacturasPagos = _uow.Clientes.BuscarTitularesFacturasPagos(propuesta.CodPropuesta)
            };

            resultado.Valor = propuestaDetalle;
            return resultado;
        }

        public Resultados<List<DtoClientesPropuestasHistorial>> DetalleHistorialTitulares(DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<List<DtoClientesPropuestasHistorial>> resultado = new();

            Resultados<Propuestas> resPropuesta = BuscarPropuesta(dtoPropuesta);
            if (resPropuesta.HayError || resPropuesta.Valor is null)
            {
                resultado.Agregar(resPropuesta);
                return resultado;
            }
            Propuestas propuesta = resPropuesta.Valor;

            resultado.Valor = _uow.Propuestas.DetalleHistorialTitulares(propuesta.CodPropuesta);
            return resultado;
        }

        private Resultados<Propuestas> BuscarPropuesta(DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<Propuestas> resultado = new();
            Propuestas? propuesta = null;
            if (dtoPropuesta.Propuesta != null)
            {
                propuesta = _uow.Propuestas.BuscarPropuesta(dtoPropuesta.Propuesta.Value);
                if (propuesta == null) resultado.Agregar("La propuesta no existe");
                else if (propuesta.FechaBaja != null && !dtoPropuesta.IncluirBaja) resultado.Agregar("La propuesta está dada de baja");

            }
            else if (!string.IsNullOrWhiteSpace(dtoPropuesta.Parcela))
            {
                propuesta = _uow.Propuestas.BuscarPropuesta(dtoPropuesta.Parcela, dtoPropuesta.IncluirBaja);
                if (propuesta == null) resultado.Agregar("La parcela no existe o no está vendida");
            }
            else
            {
                resultado.Agregar("Falta ingresar la propuesta o parcela");
            }
            resultado.Valor = propuesta;
            return resultado;
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoTitulares(DtoOpcionesListados opcionesListado)
        {
            return _uow.Propuestas.ListadoTitulares(opcionesListado);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoInhumados(DtoOpcionesListados opcionesListado)
        {
            return _uow.Propuestas.ListadoInhumados(opcionesListado);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoBeneficiarios(DtoOpcionesListados opcionesListado)
        {
            return _uow.Propuestas.ListadoBeneficiarios(opcionesListado);
        }

    }
}
