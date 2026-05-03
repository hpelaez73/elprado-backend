using ElPrado.Dto.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    /// <summary>
    /// Controlador para administrar el estado de los workers de background
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkersController : ControllerBase
    {
/*
        private readonly IWorkerStateService _workerStateService;

        public WorkersController(IWorkerStateService workerStateService)
        {
            _workerStateService = workerStateService;
        }

        [HttpGet("Estado")]
        public ActionResult<ApiResponse<string>> GetEmailWorkerStatus()
        {
            ApiResponse<string> response = new()
            {
                Success = true,
                Message = "Estado del EmailWorker obtenido correctamente",
                Data = _workerStateService.IsPaused ? "Pausado" : "Activo"
            };
            return Ok(response);
        }

        [HttpPost("Pausar")]
        public async Task<ActionResult<ApiResponse<string>>> PauseEmailWorker()
        {
            await _workerStateService.PauseAsync();
            ApiResponse<string> response = new()
            {
                Success = true,
                Message = "EmailWorker pausado correctamente",
                Data = "Pausado"
            };
            return Ok(response);
        }

        [HttpPost("Reanudar")]
        public async Task<ActionResult<ApiResponse<string>>> ResumeEmailWorker()
        {
            await _workerStateService.ResumeAsync();
            ApiResponse<string> response = new()
            {
                Success = true,
                Message = "EmailWorker reactivado correctamente",
                Data = "Activo"
            };
            return Ok(response);
        }*/
    }
}
