using ElPrado.Data.Interfaces;
using ElPrado.Services;
using ElPrado.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorBase : ControllerBase, IDisposable
    {
        private bool disposed;
        protected ServiceBase _servicio;
        protected readonly IUnitOfWork _uow;
        protected readonly IUserContextService _userContext;

        public ControladorBase(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _uow = unitOfWork;
            _userContext = userContextService;
            _servicio = CrearServicio();
        }

        protected virtual ServiceBase CrearServicio()
        {
            return new(_uow, _userContext);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DisposeServicios();
            }
            disposed = true;
        }

        protected virtual void DisposeServicios()
        {
            _servicio.Dispose();
        }
    }
}
