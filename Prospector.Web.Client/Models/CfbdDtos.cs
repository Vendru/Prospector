namespace Prospector.Web.Client.Models;

public record CfbdPlayerResult(
    int Id,
    string Name,
    string Team,
    string? Conference,
    int? Height,
    int? Weight,
    int? Jersey,
    string? Hometown
);

public record CfbdSeasonData(
    int Year,
    int Attempts,
    int Completions,
    double CompletionPct,
    int PassingYards,
    int PassingTDs,
    int Interceptions,
    double YardsPerAttempt,
    int RushingAttempts,
    int RushingYards,
    int RushingTDs,
    double? AveragePpa
);

public record CfbdImportResult(
    CfbdPlayerResult Player,
    List<CfbdSeasonData> SeasonStats
);
