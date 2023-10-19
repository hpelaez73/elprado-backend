using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElPrado.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControladorBase : ControllerBase, IDisposable
    {
        bool disposed;

        public ControladorBase()
        {
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

        }

    }
}
