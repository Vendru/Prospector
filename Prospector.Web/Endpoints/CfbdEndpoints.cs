using Prospector.Web.Services;

namespace Prospector.Web.Endpoints;

public static class CfbdEndpoints
{
    public static void MapCfbdEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cfbd").WithTags("CFBD").RequireAuthorization();

        group.MapGet("/search", async (string name, CfbdService svc) =>
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
                return Results.BadRequest("Nome deve ter pelo menos 2 caracteres.");
            var results = await svc.SearchAsync(name);
            return Results.Ok(results);
        });

        group.MapGet("/import", async (int cfbdId, string playerName, string team, CfbdService svc) =>
        {
            var result = await svc.GetImportDataAsync(cfbdId, playerName, team);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
