using ElPrado.Data;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class CampaniasService : ServiceBase
    {
        public CampaniasService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoCampaniasPropuesta(DtoOpcionesListados opcionesListado)
        {
            if (!(opcionesListado.ListFiltros?.Exists(x => x.Campo == "codUsuario") ?? false))
            {
                opcionesListado.ListFiltros ??= new();
                opcionesListado.ListFiltros.Add(
                    new DtoCamposFiltroListado
                    {
                        Campo = "codUsuario",
                        TipoComparacion = Core.Enums.TipoComparacion.Igual,
                        Valor = _userContext.GetCodUsuario().ToString()
                    }
                );
            }

            return _uow.Campanias.ListadoCampaniasPropuesta(opcionesListado);
        }
    }
}
