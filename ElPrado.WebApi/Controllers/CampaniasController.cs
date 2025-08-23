using ElPrado.Data;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class CampaniasController : ControladorBase
    {
        private CampaniasService campaniasService => (_servicio as CampaniasService)!;

        public CampaniasController(IUnitOfWork unitOfWork, IUserContextService userContextService) : base(unitOfWork, userContextService)
        {
        }

        protected override ServiceBase CrearServicio()
        {
            return new CampaniasService(_uow, _userContext);
        }

        [HttpPost("ListadoCampaniasPropuesta")]
        public ApiResponseListado<IEnumerable<dynamic>> ListadoCampaniasPropuesta([FromBody] DtoOpcionesListados opcionesListado)
        {
            return campaniasService.ListadoCampaniasPropuesta(opcionesListado);
        }

    }
}
