using System.Net.Http.Json;
using Prospector.Web.Client.Models;

namespace Prospector.Web.Client.Services;

public class PlayerApiService(HttpClient http)
{
    public async Task<PlayerPagedDto?> GetPlayersAsync(PlayerFilterRequest filter)
    {
        var query = BuildQuery(filter);
        return await http.GetFromJsonAsync<PlayerPagedDto>($"/api/players{query}");
    }

    public async Task<PlayerDetailDto?> GetPlayerAsync(int id) =>
        await http.GetFromJsonAsync<PlayerDetailDto>($"/api/players/{id}");

    public async Task<DashboardDto?> GetDashboardAsync() =>
        await http.GetFromJsonAsync<DashboardDto>("/api/dashboard");

    public async Task<IEnumerable<PlayerListDto>?> GetTopPlayersAsync(int count = 5) =>
        await http.GetFromJsonAsync<IEnumerable<PlayerListDto>>($"/api/players/top?count={count}");

    public async Task<PlayerDetailDto?> CreatePlayerAsync(CreatePlayerRequest request)
    {
        var response = await http.PostAsJsonAsync("/api/players", request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PlayerDetailDto>()
            : null;
    }

    public async Task<PlayerDetailDto?> UpdatePlayerAsync(int id, CreatePlayerRequest request)
    {
        var response = await http.PutAsJsonAsync($"/api/players/{id}", request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PlayerDetailDto>()
            : null;
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        var response = await http.DeleteAsync($"/api/players/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CfbdImportStatsAsync(int playerId, List<CfbdSeasonData> seasons)
    {
        var response = await http.PostAsJsonAsync($"/api/players/{playerId}/import-stats", seasons);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CfbdPlayerResult>> CfbdSearchAsync(string name)
    {
        try
        {
            return await http.GetFromJsonAsync<List<CfbdPlayerResult>>(
                $"/api/cfbd/search?name={Uri.EscapeDataString(name)}") ?? [];
        }
        catch { return []; }
    }

    public async Task<CfbdImportResult?> CfbdImportAsync(int cfbdId, string playerName, string team)
    {
        try
        {
            return await http.GetFromJsonAsync<CfbdImportResult>(
                $"/api/cfbd/import?cfbdId={cfbdId}&playerName={Uri.EscapeDataString(playerName)}&team={Uri.EscapeDataString(team)}");
        }
        catch { return null; }
    }

    public async Task<IEnumerable<PlayerDetailDto>?> GetPlayersForCompareAsync(IEnumerable<int> ids)
    {
        var q = string.Join("&", ids.Select(id => $"ids={id}"));
        return await http.GetFromJsonAsync<IEnumerable<PlayerDetailDto>>($"/api/players/compare?{q}");
    }

    private static string BuildQuery(PlayerFilterRequest f)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(f.Search)) parts.Add($"search={Uri.EscapeDataString(f.Search)}");
        if (f.DraftYear.HasValue) parts.Add($"draftYear={f.DraftYear}");
        if (!string.IsNullOrWhiteSpace(f.School)) parts.Add($"school={Uri.EscapeDataString(f.School)}");
        if (!string.IsNullOrWhiteSpace(f.Conference)) parts.Add($"conference={Uri.EscapeDataString(f.Conference)}");
        if (f.MinGrade.HasValue) parts.Add($"minGrade={f.MinGrade}");
        if (f.MaxGrade.HasValue) parts.Add($"maxGrade={f.MaxGrade}");
        if (f.ProjectedRound.HasValue) parts.Add($"round={f.ProjectedRound}");
        parts.Add($"sortBy={f.SortBy}");
        parts.Add($"sortDesc={f.SortDesc}");
        parts.Add($"page={f.Page}");
        parts.Add($"pageSize={f.PageSize}");
        return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
    }
}
