using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Prospector.Web.Client.Models;

namespace Prospector.Web.Services;

// Raw CFBD API models (internal, deserialization only)
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

    public async Task<CfbdImportResult?> GetImportDataAsync(string playerName, string team)
    {
        var searchResult = (await SearchAsync(playerName))
            .FirstOrDefault(p => p.Team.Equals(team, StringComparison.OrdinalIgnoreCase));
        if (searchResult is null) return null;

        var seasonStats = new List<CfbdSeasonData>();
        foreach (var year in new[] { 2022, 2023, 2024 })
        {
            var season = await BuildSeasonAsync(playerName, team, year);
            if (season is not null) seasonStats.Add(season);
        }

        return new CfbdImportResult(searchResult, seasonStats);
    }

    private async Task<CfbdSeasonData?> BuildSeasonAsync(string playerName, string team, int year)
    {
        var passing = await FetchStatsAsync(playerName, team, year, "passing");
        if (!passing.ContainsKey("ATT") || passing["ATT"] == 0) return null;

        var rushing = await FetchStatsAsync(playerName, team, year, "rushing");
        var ppa = await FetchPpaAsync(playerName, team, year);

        int att  = (int)passing.GetValueOrDefault("ATT");
        int comp = (int)passing.GetValueOrDefault("COMPLETIONS");
        int yds  = (int)passing.GetValueOrDefault("YDS");
        int td   = (int)passing.GetValueOrDefault("TD");
        int intr = (int)passing.GetValueOrDefault("INT");
        double pct = passing.GetValueOrDefault("PCT");
        double ypa = att > 0 ? Math.Round((double)yds / att, 2) : 0;

        return new CfbdSeasonData(
            year, att, comp, pct, yds, td, intr, ypa,
            (int)rushing.GetValueOrDefault("ATT"),
            (int)rushing.GetValueOrDefault("YDS"),
            (int)rushing.GetValueOrDefault("TD"),
            ppa
        );
    }

    private async Task<Dictionary<string, double>> FetchStatsAsync(string playerName, string team, int year, string category)
    {
        try
        {
            var stats = await http.GetFromJsonAsync<List<CfbdStatEntry>>(
                $"stats/player/season?year={year}&seasonType=regular&category={category}&team={Uri.EscapeDataString(team)}") ?? [];
            return stats
                .Where(s => s.Player.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(s => s.StatType, s => s.Stat);
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
                .FirstOrDefault(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                ?.AveragePpa?.All;
        }
        catch { return null; }
    }
}
