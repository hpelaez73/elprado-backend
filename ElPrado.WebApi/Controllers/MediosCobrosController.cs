using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    public class MediosCobrosController : ControladorBaseCrud<MediosCobros, DtoMediosCobros>
    {
        private MediosCobrosService mediosCobrosService => (servicio as MediosCobrosService)!;

        public MediosCobrosController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override ServiceBaseCrud<MediosCobros, DtoMediosCobros> CrearServicio()
        {
            return new MediosCobrosService(null);
        }

        [HttpPost("HistorialCobradores")]
        public ApiResponse<List<DtoHistorialCobradores>> HistorialCobradores([FromBody] DtoCuentasCorrientesReq dtoCuentas)
        {
            try
            {
                ApiResponse<List<DtoHistorialCobradores>> apiResponse = new()
                {
                    Data = mediosCobrosService.BuscarHistorialCobradores(dtoCuentas)
                };
                return apiResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "{Controlador}.HistorialCobradores({@dtoCuentas}): {Mensaje} {@Extras}", this, dtoCuentas, ex.Message, extrasLog);
                throw;
            }
        }
    }
}
