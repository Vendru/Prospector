namespace Prospector.Web.Client.Models;

public record PlayerListDto(
    int Id,
    string Name,
    string School,
    string Conference,
    string Position,
    int HeightInches,
    int WeightLbs,
    int DraftClassYear,
    int? ProjectedRound,
    int? ProjectedPick,
    double? OverallGrade,
    double? CeilingGrade,
    double? FloorGrade,
    string? NflComparison,
    int? JerseyNumber,
    LatestStatsDto? LatestStats
);

public record LatestStatsDto(
    int Year,
    int GamesPlayed,
    int Attempts,
    int Completions,
    double CompletionPct,
    int PassingYards,
    int PassingTDs,
    int Interceptions,
    double YardsPerAttempt,
    int RushingYards,
    int RushingTDs,
    double? Qbr,
    double? Epa,
    double? Cpoe
);

public record PlayerDetailDto(
    int Id,
    string Name,
    string School,
    string Conference,
    string Position,
    int HeightInches,
    int WeightLbs,
    int DraftClassYear,
    int? ProjectedRound,
    int? ProjectedPick,
    double? OverallGrade,
    double? CeilingGrade,
    double? FloorGrade,
    string? NflComparison,
    int? JerseyNumber,
    string? Bio,
    string HomeTown,
    string HomeState,
    IEnumerable<QBStatsDto> SeasonStats,
    ScoutGradeDto? ScoutGrade,
    PhysicalMeasurementDto? LatestMeasurement
);

public record QBStatsDto(
    int Id,
    int Year,
    string Season,
    int GamesPlayed,
    int GamesStarted,
    int Attempts,
    int Completions,
    double CompletionPct,
    int PassingYards,
    int PassingTDs,
    int Interceptions,
    int Sacks,
    double YardsPerAttempt,
    double AdjYardsPerAttempt,
    double? PasserRating,
    int RushingAttempts,
    int RushingYards,
    int RushingTDs,
    double? Qbr,
    double? Epa,
    double? EpaPerPlay,
    double? Cpoe,
    double? AirYardsPerAttempt,
    double? OnTargetThrowPct,
    double? PressureRate,
    double? SackRate,
    double? ThirdDownConvPct,
    double? RedZoneCompPct,
    double? DropPct
);

public record ScoutGradeDto(
    double ArmStrength,
    double Accuracy,
    double DeepBallAccuracy,
    double Footwork,
    double Mechanics,
    double DecisionMaking,
    double PocketPresence,
    double MobilityAgility,
    double Leadership,
    double FootballIq,
    double ReleaseSpeed,
    string? Strengths,
    string? Weaknesses,
    string? NflComparison
);

public record PhysicalMeasurementDto(
    string Event,
    double? FortyYardDash,
    double? VerticalJump,
    double? BroadJump,
    double? ThreeConeDrill,
    double? ShuttleRun,
    double? HandSizeInches,
    double? ArmLengthInches,
    double? WingSpanInches
);
