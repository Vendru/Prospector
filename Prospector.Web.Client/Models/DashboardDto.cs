namespace Prospector.Web.Client.Models;

public record DashboardDto(
    int TotalProspects,
    Dictionary<int, int> ProspectsByDraftClass,
    IEnumerable<PlayerListDto> TopRatedPlayers,
    IEnumerable<PlayerListDto> TopByQbr,
    IEnumerable<PlayerListDto> TopByEpa,
    IEnumerable<PlayerListDto> RecentlyAdded,
    double AverageOverallGrade,
    double AverageCeilingGrade
);

public record PlayerPagedDto(
    IEnumerable<PlayerListDto> Players,
    int Total,
    int Page,
    int PageSize,
    int TotalPages
);

public class PlayerFilterRequest
{
    public string? Search { get; set; }
    public int? DraftYear { get; set; }
    public string? School { get; set; }
    public string? Conference { get; set; }
    public int? MinGrade { get; set; }
    public int? MaxGrade { get; set; }
    public int? ProjectedRound { get; set; }
    public string SortBy { get; set; } = "OverallGrade";
    public bool SortDesc { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public record UserInfoDto(
    bool IsAuthenticated,
    string? Email,
    string? DisplayName,
    string[]? Roles
);

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; } = true;
}

public class RegisterRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string DisplayName { get; set; } = "";
}
public record AuthResult(bool Success, string? Message);

public class CreatePlayerRequest
{
    public string Name { get; set; } = "";
    public string School { get; set; } = "";
    public string Conference { get; set; } = "";
    public string Position { get; set; } = "QB";
    public int DraftClassYear { get; set; } = DateTime.Now.Year + 1;
    public int HeightInches { get; set; }
    public int WeightLbs { get; set; }
    public string HomeTown { get; set; } = "";
    public string HomeState { get; set; } = "";
    public string? Bio { get; set; }
    public string? NflComparison { get; set; }
    public int? JerseyNumber { get; set; }
    public int? ProjectedRound { get; set; }
    public int? ProjectedPick { get; set; }
    public double? OverallGrade { get; set; }
    public double? CeilingGrade { get; set; }
    public double? FloorGrade { get; set; }
}
