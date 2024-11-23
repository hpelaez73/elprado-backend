using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class PropuestasService : ServiceBase
    {
        private PropuestasRepository propuestasRepository => (repository as PropuestasRepository)!;

        public PropuestasService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new PropuestasRepository(Transaccion);
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

            ClientesRepository clientesRepository = new(Transaccion);
            resultado.Valor = clientesRepository.BuscarTitulares(propuesta.CodPropuesta);

            return resultado;
        }

        public Resultados<DtoPropuestaDetalleResp> Detalle (DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<DtoPropuestaDetalleResp> resultado = new();

            Resultados<Propuestas> resPropuesta = BuscarPropuesta(dtoPropuesta);
            if (resPropuesta.HayError || resPropuesta.Valor is null)
            {
                resultado.Agregar(resPropuesta);
                return resultado;
            }
            Propuestas propuesta = resPropuesta.Valor;
            DtoPropuestaDetalleResp? propuestaDetalle = propuestasRepository.BuscarPropuestaDetalle(propuesta.CodPropuesta);

            if (propuestaDetalle != null)
            {
                propuestaDetalle.ListPropuestasAsociadas = propuestasRepository.BuscarPropuestasAsociadas(propuestaDetalle.CodPropuesta, dtoPropuesta.IncluirBaja);

                if (propuestaDetalle.CodParcela != null)
                {
                    ParcelasRepository parcelasRepository = new(Transaccion);
                    propuestaDetalle.ListZonasParcelas = parcelasRepository.BuscarZonasParcelas(propuestaDetalle.CodParcela.Value);
                    propuestaDetalle.ListDetalleLugares = parcelasRepository.BuscarDetalleLugares(propuestaDetalle.CodParcela.Value, propuestaDetalle.CodPropuesta);

                    InhumadosRepository inhumadosRepository = new(Transaccion);
                    propuestaDetalle.ListInhumados = inhumadosRepository.BuscarInhumados(propuestaDetalle.CodPropuesta, propuestaDetalle.CodParcela.Value);
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

            DtoPropuestaDetalleContratosResp propuestaDetalle = new();

            ContratosRepository contratosRepository = new(Transaccion);
            propuestaDetalle.ListContratos = contratosRepository.BuscarContratos(propuesta.CodPropuesta, dtoPropuesta.IncluirBaja);

            PlanesVentasRepository planesVentasRepository = new(Transaccion);
            propuestaDetalle.ListPlanesVentas = planesVentasRepository.BuscarPlanesVentas(propuesta.CodPropuesta, dtoPropuesta.IncluirBaja);

            ClientesRepository clientesRepository = new(Transaccion);
            propuestaDetalle.ListTitulares = clientesRepository.BuscarTitulares(propuesta.CodPropuesta);
            propuestaDetalle.ListTitularesFacturasPagos = clientesRepository.BuscarTitularesFacturasPagos(propuesta.CodPropuesta);

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

            resultado.Valor = propuestasRepository.DetalleHistorialTitulares(propuesta.CodPropuesta);
            return resultado;
        }

        private Resultados<Propuestas> BuscarPropuesta(DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<Propuestas> resultado = new();
            Propuestas? propuesta = null;
            if (dtoPropuesta.Propuesta != null)
            {
                propuesta = propuestasRepository.BuscarPropuesta(dtoPropuesta.Propuesta.Value);
                if (propuesta == null) resultado.Agregar("La propuesta no existe");
                else if (propuesta.FechaBaja != null && !dtoPropuesta.IncluirBaja) resultado.Agregar("La propuesta está dada de baja");

            }
            else if (!string.IsNullOrWhiteSpace(dtoPropuesta.Parcela))
            {
                propuesta = propuestasRepository.BuscarPropuesta(dtoPropuesta.Parcela, dtoPropuesta.IncluirBaja);
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
            return propuestasRepository.ListadoTitulares(opcionesListado);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoInhumados(DtoOpcionesListados opcionesListado)
        {
            return propuestasRepository.ListadoInhumados(opcionesListado);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoBeneficiarios(DtoOpcionesListados opcionesListado)
        {
            return propuestasRepository.ListadoBeneficiarios(opcionesListado);
        }

    }
}
