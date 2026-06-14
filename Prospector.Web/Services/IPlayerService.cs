using Prospector.Web.Models;

namespace Prospector.Web.Services;

public interface IPlayerService
{
    Task<(IEnumerable<Player> Players, int Total)> GetPlayersAsync(PlayerFilter filter);
    Task<Player?> GetPlayerByIdAsync(int id);
    Task<Player> CreatePlayerAsync(Player player);
    Task<Player?> UpdatePlayerAsync(int id, Player player);
    Task<bool> DeletePlayerAsync(int id);
    Task<IEnumerable<Player>> GetTopByGradeAsync(int count = 5);
    Task<IEnumerable<Player>> GetTopByQbrAsync(int count = 5);
    Task<IEnumerable<Player>> GetTopByEpaAsync(int count = 5);
    Task<Dictionary<int, int>> GetProspectsByDraftClassAsync();
    Task<IEnumerable<Player>> GetRecentlyAddedAsync(int count = 5);
    Task ImportSeasonStatsAsync(int playerId, IEnumerable<Prospector.Web.Client.Models.CfbdSeasonData> seasons);
}

public record PlayerFilter(
    string? Search = null,
    int? DraftYear = null,
    string? School = null,
    string? Conference = null,
    int? MinGrade = null,
    int? MaxGrade = null,
    int? ProjectedRound = null,
    string SortBy = "OverallGrade",
    bool SortDesc = true,
    int Page = 1,
    int PageSize = 12
);
