using ElPrado.Core;
using ElPrado.Core.Utils;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ParcelasService : ServiceBase
    {
        public ParcelasService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public Resultados<DtoCoordenadas> BuscarCoordenada(DtoParcelasCoordenadasReq dtoParcela)
        {
            Resultados<DtoCoordenadas> resultado = new();

            if (!string.IsNullOrEmpty(dtoParcela.NroTelefono))
            {
                if (!FunUtils.EsTelefonoValido(dtoParcela.NroTelefono)) resultado.Agregar("El número de telefono no tiene el formato correcto");
            }
            if (resultado.HayError) return resultado;

            resultado.Valor = _uow.Parcelas.BuscarCoordenadas(dtoParcela.CodParcela, dtoParcela.NroTelefono!);
            return resultado;
        }
    }
}