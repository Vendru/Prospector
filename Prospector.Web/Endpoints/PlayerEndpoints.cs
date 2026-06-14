using Prospector.Web.Client.Models;
using Prospector.Web.Services;

namespace Prospector.Web.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/players").WithTags("Players");

        group.MapGet("/", GetPlayers);
        group.MapGet("/top", GetTopPlayers);
        group.MapGet("/compare", GetPlayersForCompare);
        group.MapGet("/{id:int}", GetPlayer);
        group.MapPost("/", CreatePlayer).RequireAuthorization();
        group.MapPost("/{id:int}/import-stats", ImportStats).RequireAuthorization();
        group.MapPut("/{id:int}", UpdatePlayer).RequireAuthorization(p => p.RequireRole("Admin"));
        group.MapDelete("/{id:int}", DeletePlayer).RequireAuthorization(p => p.RequireRole("Admin"));
    }

    private static async Task<IResult> GetPlayers(
        IPlayerService svc,
        string? search, int? draftYear, string? school, string? conference,
        int? minGrade, int? maxGrade, int? round,
        string sortBy = "OverallGrade", bool sortDesc = true, int page = 1, int pageSize = 12)
    {
        var filter = new PlayerFilter(search, draftYear, school, conference, minGrade, maxGrade, round, sortBy, sortDesc, page, pageSize);
        var (players, total) = await svc.GetPlayersAsync(filter);
        var dtos = players.Select(p => MapToListDto(p));
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return Results.Ok(new PlayerPagedDto(dtos, total, page, pageSize, totalPages));
    }

    private static async Task<IResult> GetTopPlayers(IPlayerService svc, int count = 5)
    {
        var players = await svc.GetTopByGradeAsync(count);
        return Results.Ok(players.Select(MapToListDto));
    }

    private static async Task<IResult> GetPlayersForCompare(IPlayerService svc, [Microsoft.AspNetCore.Mvc.FromQuery] int[] ids)
    {
        var tasks = ids.Select(id => svc.GetPlayerByIdAsync(id));
        var players = await Task.WhenAll(tasks);
        var dtos = players.Where(p => p != null).Select(p => MapToDetailDto(p!));
        return Results.Ok(dtos);
    }

    private static async Task<IResult> GetPlayer(int id, IPlayerService svc)
    {
        var player = await svc.GetPlayerByIdAsync(id);
        return player is null ? Results.NotFound() : Results.Ok(MapToDetailDto(player));
    }

    private static async Task<IResult> CreatePlayer(Prospector.Web.Models.Player player, IPlayerService svc)
    {
        var created = await svc.CreatePlayerAsync(player);
        return Results.Created($"/api/players/{created.Id}", MapToDetailDto(created));
    }

    private static async Task<IResult> ImportStats(
        int id,
        List<Prospector.Web.Client.Models.CfbdSeasonData> seasons,
        IPlayerService svc)
    {
        await svc.ImportSeasonStatsAsync(id, seasons);
        return Results.Ok();
    }

    private static async Task<IResult> UpdatePlayer(int id, Prospector.Web.Models.Player player, IPlayerService svc)
    {
        var updated = await svc.UpdatePlayerAsync(id, player);
        return updated is null ? Results.NotFound() : Results.Ok(MapToDetailDto(updated));
    }

    private static async Task<IResult> DeletePlayer(int id, IPlayerService svc)
    {
        var deleted = await svc.DeletePlayerAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    // --- Mapping helpers ---

    internal static PlayerListDto MapToListDto(Prospector.Web.Models.Player p)
    {
        var latest = p.SeasonStats.MaxBy(s => s.Year);
        LatestStatsDto? statsDto = latest is null ? null : new(
            latest.Year, latest.GamesPlayed, latest.Attempts, latest.Completions,
            latest.CompletionPct, latest.PassingYards, latest.PassingTDs,
            latest.Interceptions, latest.YardsPerAttempt, latest.RushingYards,
            latest.RushingTDs, latest.Qbr, latest.Epa, latest.Cpoe
        );
        return new(p.Id, p.Name, p.School, p.Conference, p.Position,
            p.HeightInches, p.WeightLbs, p.DraftClassYear,
            p.ProjectedRound, p.ProjectedPick,
            p.OverallGrade, p.CeilingGrade, p.FloorGrade,
            p.NflComparison, p.JerseyNumber, statsDto);
    }

    internal static PlayerDetailDto MapToDetailDto(Prospector.Web.Models.Player p)
    {
        var statsDtos = p.SeasonStats.OrderByDescending(s => s.Year).Select(s => new QBStatsDto(
            s.Id, s.Year, s.Season, s.GamesPlayed, s.GamesStarted, s.Attempts, s.Completions,
            s.CompletionPct, s.PassingYards, s.PassingTDs, s.Interceptions, s.Sacks,
            s.YardsPerAttempt, s.AdjYardsPerAttempt, s.PasserRating,
            s.RushingAttempts, s.RushingYards, s.RushingTDs,
            s.Qbr, s.Epa, s.EpaPerPlay, s.Cpoe, s.AirYardsPerAttempt,
            s.OnTargetThrowPct, s.PressureRate, s.SackRate,
            s.ThirdDownConvPct, s.RedZoneCompPct, s.DropPct
        ));

        var scout = p.ScoutReports.MaxBy(r => r.ReportDate);
        ScoutGradeDto? gradeDto = scout is null ? null : new(
            scout.ArmStrength, scout.Accuracy, scout.DeepBallAccuracy, scout.Footwork,
            scout.Mechanics, scout.DecisionMaking, scout.PocketPresence, scout.MobilityAgility,
            scout.Leadership, scout.FootballIq, scout.ReleaseSpeed,
            scout.Strengths, scout.Weaknesses, scout.NflComparison
        );

        var meas = p.Measurements.MaxBy(m => m.MeasuredAt);
        PhysicalMeasurementDto? measDto = meas is null ? null : new(
            meas.Event, meas.FortyYardDash, meas.VerticalJump, meas.BroadJump,
            meas.ThreeConeDrill, meas.ShuttleRun, meas.HandSizeInches,
            meas.ArmLengthInches, meas.WingSpanInches
        );

        return new(p.Id, p.Name, p.School, p.Conference, p.Position,
            p.HeightInches, p.WeightLbs, p.DraftClassYear,
            p.ProjectedRound, p.ProjectedPick,
            p.OverallGrade, p.CeilingGrade, p.FloorGrade,
            p.NflComparison, p.JerseyNumber, p.Bio,
            p.HomeTown, p.HomeState, statsDtos, gradeDto, measDto);
    }
}
