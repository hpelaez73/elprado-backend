using ElPrado.Data.Interfaces;
using ElPrado.McpApi.Auth;
using ElPrado.McpApi.Contracts;
using ElPrado.Services;
using ElPrado.Services.Services;

namespace ElPrado.McpApi.Endpoints;

public static class LoginEndpointExtensions
{
    public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/login/usuario", (
            LoginUsuarioRequest request,
            IUnitOfWork unitOfWork,
            IUserContextService userContext,
            McpJwtTokenIssuer tokenIssuer) =>
        {
            using LoginService loginService = new(unitOfWork, userContext);
            var login = loginService.AutenticarUsuario(request.Alias, request.Clave);
            if (login == null)
            {
                return Results.Unauthorized();
            }

            IssuedAccessToken accessToken = tokenIssuer.Issue(login);
            return Results.Ok(ApiResponse<LoginUsuarioResponse>.Success(new LoginUsuarioResponse(
                login.CodUsuario,
                login.Nombre,
                accessToken.Token,
                accessToken.ExpiresAt)));
        }).AllowAnonymous();

        return endpoints;
    }
}
