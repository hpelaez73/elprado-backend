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

        public Resultados<DtoPropuestaDetalleResp> Detalle (DtoPropuestaDetalleReq dtoPropuesta)
        {
            Resultados<DtoPropuestaDetalleResp> resultado = new();

            Propuestas? propuesta;
            if (dtoPropuesta.Propuesta != null)
            {
                propuesta = propuestasRepository.BuscarPropuesta(dtoPropuesta.Propuesta.Value);
                if (propuesta == null)
                {
                    resultado.Agregar("La propuesta no existe");
                    return resultado;
                }
                else if (propuesta.FechaBaja != null && !dtoPropuesta.IncluirBaja)
                {
                    resultado.Agregar("La propuesta está dada de baja");
                    return resultado;
                }
            }
            else if (!string.IsNullOrWhiteSpace(dtoPropuesta.Parcela))
            {
                propuesta = propuestasRepository.BuscarPropuesta(dtoPropuesta.Parcela, dtoPropuesta.IncluirBaja);
                if (propuesta == null)
                {
                    resultado.Agregar("La parcela no existe o no está vendida");
                    return resultado;
                }
            }
            else
            {
                resultado.Agregar("Falta ingresar la propuesta o parcela");
                return resultado;
            }

            DtoPropuestaDetalleResp? propuestaDetalle = propuestasRepository.BuscarPropuestaDetalle(propuesta.CodPropuesta);

            if (propuestaDetalle != null && propuestaDetalle.CodParcela != null)
            {
                propuestaDetalle.ListPropuestasAsociadas = propuestasRepository.BuscarPropuestasAsociadas(propuestaDetalle.CodPropuesta, dtoPropuesta.IncluirBaja);

                ParcelasRepository parcelasRepository = new(Transaccion);
                propuestaDetalle.ListZonasParcelas = parcelasRepository.BuscarZonasParcelas(propuestaDetalle.CodParcela.Value);
                propuestaDetalle.ListDetalleLugares = parcelasRepository.BuscarDetalleLugares(propuestaDetalle.CodParcela.Value, propuestaDetalle.CodPropuesta);

                InhumadosRepository inhumadosRepository = new(Transaccion);
                propuestaDetalle.ListInhumados = inhumadosRepository.BuscarInhumados(propuestaDetalle.CodPropuesta, propuestaDetalle.CodParcela.Value);

                ClientesRepository clientesRepository = new(Transaccion);
                propuestaDetalle.ListTitulares = clientesRepository.BuscarTitulares(propuestaDetalle.CodPropuesta);
            }

            resultado.Valor = propuestaDetalle;

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
