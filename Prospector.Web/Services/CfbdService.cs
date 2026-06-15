using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Prospector.Web.Client.Models;

namespace Prospector.Web.Services;

file record CfbdSearchEntry(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("team")] string Team,
    [property: JsonPropertyName("position")] string? Position,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("weight")] int? Weight,
    [property: JsonPropertyName("jersey")] int? Jersey,
    [property: JsonPropertyName("hometown")] string? Hometown
);

file record CfbdStatEntry(
    [property: JsonPropertyName("player")] string Player,
    [property: JsonPropertyName("team")] string Team,
    [property: JsonPropertyName("conference")] string? Conference,
    [property: JsonPropertyName("statType")] string StatType,
    [property: JsonPropertyName("stat")] double Stat
);

file record CfbdPpaEntry(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("team")] string Team,
    [property: JsonPropertyName("conference")] string? Conference,
    [property: JsonPropertyName("averagePPA")] CfbdAvgPpa? AveragePpa
);

file record CfbdAvgPpa(
    [property: JsonPropertyName("all")] double? All
);

public class CfbdService(HttpClient http)
{
    public async Task<List<CfbdPlayerResult>> SearchAsync(string name)
    {
        try
        {
            var results = await http.GetFromJsonAsync<List<CfbdSearchEntry>>(
                $"player/search?searchTerm={Uri.EscapeDataString(name)}&position=QB") ?? [];

            return results.Select(r => new CfbdPlayerResult(
                r.Id, r.Name, r.Team, null, r.Height, r.Weight, r.Jersey, r.Hometown
            )).Take(8).ToList();
        }
        catch { return []; }
    }

    public async Task<CfbdImportResult?> GetImportDataAsync(int cfbdId, string playerName, string team)
    {
        // Fetch last 4 seasons in parallel (passing + rushing + PPA for each year)
        var currentYear = DateTime.Now.Year;
        var years = Enumerable.Range(currentYear - 4, 4).ToArray();

        var seasonTasks = years.Select(year => BuildSeasonAsync(cfbdId, playerName, team, year));
        var seasons = await Task.WhenAll(seasonTasks);

        var seasonStats = seasons.Where(s => s is not null).Cast<CfbdSeasonData>().ToList();

        // Try to get conference from PPA endpoint
        var conference = await GetConferenceAsync(playerName, team, currentYear - 1);

        var player = new CfbdPlayerResult(cfbdId, playerName, team, conference, null, null, null, null);
        return new CfbdImportResult(player, seasonStats);
    }

    private async Task<CfbdSeasonData?> BuildSeasonAsync(int cfbdId, string playerName, string team, int year)
    {
        // Fetch passing, rushing, and PPA in parallel
        var passingTask = FetchStatsAsync(cfbdId, playerName, team, year, "passing");
        var rushingTask = FetchStatsAsync(cfbdId, playerName, team, year, "rushing");
        var ppaTask     = FetchPpaAsync(playerName, team, year);

        await Task.WhenAll(passingTask, rushingTask, ppaTask);

        var passing = passingTask.Result;
        var rushing = rushingTask.Result;
        var ppa     = ppaTask.Result;

        if (!passing.ContainsKey("ATT") || passing["ATT"] == 0) return null;

        int att  = (int)passing.GetValueOrDefault("ATT");
        int comp = (int)passing.GetValueOrDefault("COMPLETIONS");
        int yds  = (int)passing.GetValueOrDefault("YDS");
        int td   = (int)passing.GetValueOrDefault("TD");
        int intr = (int)passing.GetValueOrDefault("INT");
        double pct = comp > 0 && att > 0 ? Math.Round((double)comp / att * 100, 1) : passing.GetValueOrDefault("PCT");
        double ypa = att > 0 ? Math.Round((double)yds / att, 2) : 0;

        return new CfbdSeasonData(
            year, att, comp, pct, yds, td, intr, ypa,
            (int)rushing.GetValueOrDefault("ATT"),
            (int)rushing.GetValueOrDefault("YDS"),
            (int)rushing.GetValueOrDefault("TD"),
            ppa
        );
    }

    private async Task<Dictionary<string, double>> FetchStatsAsync(int cfbdId, string playerName, string team, int year, string category)
    {
        try
        {
            // Try by athleteId first (more accurate), fall back to team filter
            var url = $"stats/player/season?year={year}&seasonType=regular&category={category}&team={Uri.EscapeDataString(team)}";
            var stats = await http.GetFromJsonAsync<List<CfbdStatEntry>>(url) ?? [];

            // Match by name (case-insensitive, handles "First Last" vs "Last, First")
            var playerStats = stats
                .Where(s => NamesMatch(s.Player, playerName))
                .ToDictionary(s => s.StatType, s => s.Stat);

            return playerStats;
        }
        catch { return []; }
    }

    private async Task<double?> FetchPpaAsync(string playerName, string team, int year)
    {
        try
        {
            var ppas = await http.GetFromJsonAsync<List<CfbdPpaEntry>>(
                $"metrics/ppa/players/season?year={year}&seasonType=regular&team={Uri.EscapeDataString(team)}") ?? [];
            return ppas
                .FirstOrDefault(p => NamesMatch(p.Name, playerName))
                ?.AveragePpa?.All;
        }
        catch { return null; }
    }

    private async Task<string?> GetConferenceAsync(string playerName, string team, int year)
    {
        try
        {
            var ppas = await http.GetFromJsonAsync<List<CfbdPpaEntry>>(
                $"metrics/ppa/players/season?year={year}&seasonType=regular&team={Uri.EscapeDataString(team)}") ?? [];
            return ppas.FirstOrDefault(p => NamesMatch(p.Name, playerName))?.Conference;
        }
        catch { return null; }
    }

    private static bool NamesMatch(string a, string b) =>
        a.Equals(b, StringComparison.OrdinalIgnoreCase) ||
        a.Replace(" ", "").Equals(b.Replace(" ", ""), StringComparison.OrdinalIgnoreCase);
}
