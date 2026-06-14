using Microsoft.EntityFrameworkCore;
using Prospector.Web.Data;
using Prospector.Web.Models;

namespace Prospector.Web.Services;

public class PlayerDbService(ApplicationDbContext db) : IPlayerService
{
    public async Task<(IEnumerable<Player> Players, int Total)> GetPlayersAsync(PlayerFilter filter)
    {
        var query = db.Players
            .Include(p => p.SeasonStats.OrderByDescending(s => s.Year).Take(1))
            .Include(p => p.ScoutReports.Take(1))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var q = filter.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(q) || p.School.ToLower().Contains(q));
        }
        if (filter.DraftYear.HasValue)
            query = query.Where(p => p.DraftClassYear == filter.DraftYear.Value);
        if (!string.IsNullOrWhiteSpace(filter.School))
            query = query.Where(p => p.School == filter.School);
        if (!string.IsNullOrWhiteSpace(filter.Conference))
            query = query.Where(p => p.Conference == filter.Conference);
        if (filter.MinGrade.HasValue)
            query = query.Where(p => p.OverallGrade >= filter.MinGrade.Value);
        if (filter.MaxGrade.HasValue)
            query = query.Where(p => p.OverallGrade <= filter.MaxGrade.Value);
        if (filter.ProjectedRound.HasValue)
            query = query.Where(p => p.ProjectedRound == filter.ProjectedRound.Value);

        query = (filter.SortBy, filter.SortDesc) switch
        {
            ("Name", false) => query.OrderBy(p => p.Name),
            ("Name", true) => query.OrderByDescending(p => p.Name),
            ("School", false) => query.OrderBy(p => p.School),
            ("School", true) => query.OrderByDescending(p => p.School),
            ("CeilingGrade", false) => query.OrderBy(p => p.CeilingGrade),
            ("CeilingGrade", true) => query.OrderByDescending(p => p.CeilingGrade),
            ("DraftClassYear", false) => query.OrderBy(p => p.DraftClassYear),
            ("DraftClassYear", true) => query.OrderByDescending(p => p.DraftClassYear),
            (_, false) => query.OrderBy(p => p.OverallGrade),
            _ => query.OrderByDescending(p => p.OverallGrade),
        };

        var total = await query.CountAsync();
        var players = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (players, total);
    }

    public async Task<Player?> GetPlayerByIdAsync(int id) =>
        await db.Players
            .Include(p => p.SeasonStats.OrderByDescending(s => s.Year))
            .Include(p => p.ScoutReports)
            .Include(p => p.Measurements)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Player> CreatePlayerAsync(Player player)
    {
        player.CreatedAt = DateTime.UtcNow;
        player.UpdatedAt = DateTime.UtcNow;
        db.Players.Add(player);
        await db.SaveChangesAsync();
        return player;
    }

    public async Task<Player?> UpdatePlayerAsync(int id, Player updated)
    {
        var player = await db.Players.FindAsync(id);
        if (player is null) return null;

        player.Name = updated.Name;
        player.School = updated.School;
        player.Conference = updated.Conference;
        player.DraftClassYear = updated.DraftClassYear;
        player.HeightInches = updated.HeightInches;
        player.WeightLbs = updated.WeightLbs;
        player.HomeTown = updated.HomeTown;
        player.HomeState = updated.HomeState;
        player.Bio = updated.Bio;
        player.ProjectedRound = updated.ProjectedRound;
        player.ProjectedPick = updated.ProjectedPick;
        player.OverallGrade = updated.OverallGrade;
        player.CeilingGrade = updated.CeilingGrade;
        player.FloorGrade = updated.FloorGrade;
        player.NflComparison = updated.NflComparison;
        player.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return player;
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        var player = await db.Players.FindAsync(id);
        if (player is null) return false;
        db.Players.Remove(player);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task ImportSeasonStatsAsync(int playerId, IEnumerable<Prospector.Web.Client.Models.CfbdSeasonData> seasons)
    {
        foreach (var s in seasons)
        {
            db.Set<QBSeasonStats>().Add(new QBSeasonStats
            {
                PlayerId = playerId,
                Year = s.Year,
                Season = "Regular",
                Attempts = s.Attempts,
                Completions = s.Completions,
                CompletionPct = s.CompletionPct,
                PassingYards = s.PassingYards,
                PassingTDs = s.PassingTDs,
                Interceptions = s.Interceptions,
                YardsPerAttempt = s.YardsPerAttempt,
                RushingAttempts = s.RushingAttempts,
                RushingYards = s.RushingYards,
                RushingTDs = s.RushingTDs,
                EpaPerPlay = s.AveragePpa,
            });
        }
        await db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Player>> GetTopByGradeAsync(int count = 5) =>
        await db.Players
            .Where(p => p.OverallGrade.HasValue)
            .OrderByDescending(p => p.OverallGrade)
            .Take(count)
            .Include(p => p.SeasonStats.OrderByDescending(s => s.Year).Take(1))
            .ToListAsync();

    public async Task<IEnumerable<Player>> GetTopByQbrAsync(int count = 5)
    {
        var players = await db.Players
            .Include(p => p.SeasonStats.OrderByDescending(s => s.Year).Take(1))
            .ToListAsync();
        return players
            .Where(p => p.SeasonStats.Any(s => s.Qbr.HasValue))
            .OrderByDescending(p => p.SeasonStats.Max(s => s.Qbr ?? 0))
            .Take(count);
    }

    public async Task<IEnumerable<Player>> GetTopByEpaAsync(int count = 5)
    {
        var players = await db.Players
            .Include(p => p.SeasonStats.OrderByDescending(s => s.Year).Take(1))
            .ToListAsync();
        return players
            .Where(p => p.SeasonStats.Any(s => s.Epa.HasValue))
            .OrderByDescending(p => p.SeasonStats.Max(s => s.Epa ?? 0))
            .Take(count);
    }

    public async Task<Dictionary<int, int>> GetProspectsByDraftClassAsync() =>
        await db.Players
            .GroupBy(p => p.DraftClassYear)
            .Select(g => new { Year = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Year, x => x.Count);

    public async Task<IEnumerable<Player>> GetRecentlyAddedAsync(int count = 5) =>
        await db.Players
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
}
