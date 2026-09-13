namespace ElPrado.McpApi.Endpoints;

/// <summary>
/// Entry point for future thin HTTP adapters. Adapters validate their request,
/// delegate business behavior to shared services, and map only this API's contract.
/// </summary>
public static class BusinessEndpointExtensions
{
    public static RouteGroupBuilder MapBusinessOperations(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api").RequireAuthorization();
        group.MapProposalOperations();
        return group;
    }
}
