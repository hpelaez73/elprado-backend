using ElPrado.Services;
using Serilog;
using Serilog.Context;
using System.Text.Json;

namespace ElPrado.WebApi.MiddleWares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserContextService userContext)
        {
            {
                // Habilita el buffering del body para que sea seekable
                context.Request.EnableBuffering();

                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    var routeData = context.GetRouteData();
                    var controller = routeData.Values["controller"]?.ToString();
                    var action = routeData.Values["action"]?.ToString();

                    var queryParams = routeData.Values
                        .Where(kv => kv.Key != "controller" && kv.Key != "action")
                        .Select(kv => new Tuple<string, string>(kv.Key, kv.Value?.ToString() ?? string.Empty))
                        .ToArray();

                    // Parámetros del body (si es JSON)
                    string bodyParams = string.Empty;
                    if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
                    {
                        context.Request.Body.Position = 0;
                        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                        bodyParams = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0;
                    }

                    var extrasLog = new
                    {
                        CodUsuario = userContext.GetCodUsuario(),
                        CodCliente = userContext.GetCodCliente(),
                        CodPropuesta = userContext.GetCodPropuesta(),
                        QueryParams = queryParams,
                        BodyParams = bodyParams
                    };

                    Log.Error(ex, "{Controlador}.{Accion}: {Mensaje} {@Extras}", controller, action, ex.Message, extrasLog);

                    await HandleExceptionAsync(context, ex);
                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int code;
            string errorMessage;

            switch (exception)
            {
                case ArgumentException argEx:
                    code = StatusCodes.Status400BadRequest;
                    errorMessage = argEx.Message;
                    break;
                case UnauthorizedAccessException unauthEx:
                    code = StatusCodes.Status401Unauthorized;
                    errorMessage = "No autorizado.";
                    break;
                // Puedes agregar tus propias excepciones personalizadas aquí
                // case MiExcepcionPersonalizada miEx:
                //     code = StatusCodes.Status409Conflict;
                //     errorMessage = miEx.Message;
                //     break;
                default:
                    code = StatusCodes.Status500InternalServerError;
                    errorMessage = "Ha ocurrido un error inesperado.";
                    break;
            }

            var result = JsonSerializer.Serialize(new { error = errorMessage });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            return context.Response.WriteAsync(result);
        }
    }
}
