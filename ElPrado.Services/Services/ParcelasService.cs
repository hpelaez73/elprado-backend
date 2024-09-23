using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ParcelasService : ServiceBase
    {
        private ParcelasRepository parcelasRepository => (repository as ParcelasRepository)!;

        public ParcelasService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new ParcelasRepository(Transaccion);
        }

        public Resultados<DtoCoordenadas> BuscarCoordenada(DtoParcelasCoordenadasReq dtoParcela)
        {
            Resultados<DtoCoordenadas> resultado = new();

            if (!string.IsNullOrEmpty(dtoParcela.NroTelefono))
            {
                if (!Utils.EsTelefonoValido(dtoParcela.NroTelefono)) resultado.Agregar("El número de telefono no tiene el formato correcto");                 
            }
            if (resultado.HayError) return resultado;

            resultado.Valor = parcelasRepository.BuscarCoordenadas(dtoParcela.CodParcela, dtoParcela.NroTelefono!);
            return resultado;
        }
    }
}