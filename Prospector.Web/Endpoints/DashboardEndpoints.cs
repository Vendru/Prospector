using Prospector.Web.Client.Models;
using Prospector.Web.Services;

namespace Prospector.Web.Endpoints;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/dashboard", GetDashboard).WithTags("Dashboard");
    }

    private static async Task<IResult> GetDashboard(IPlayerService svc)
    {
        var topRated = (await svc.GetTopByGradeAsync(5)).Select(PlayerEndpoints.MapToListDto).ToList();
        var topQbr = (await svc.GetTopByQbrAsync(5)).Select(PlayerEndpoints.MapToListDto).ToList();
        var topEpa = (await svc.GetTopByEpaAsync(5)).Select(PlayerEndpoints.MapToListDto).ToList();
        var recent = (await svc.GetRecentlyAddedAsync(5)).Select(PlayerEndpoints.MapToListDto).ToList();
        var byClass = await svc.GetProspectsByDraftClassAsync();

        var (allPlayers, total) = await svc.GetPlayersAsync(new PlayerFilter(PageSize: 1000));
        var avgOverall = allPlayers.Where(p => p.OverallGrade.HasValue).Average(p => p.OverallGrade!.Value);
        var avgCeiling = allPlayers.Where(p => p.CeilingGrade.HasValue).Average(p => p.CeilingGrade!.Value);

        return Results.Ok(new DashboardDto(
            total, byClass, topRated, topQbr, topEpa, recent,
            Math.Round(avgOverall, 1), Math.Round(avgCeiling, 1)
        ));
    }
}
