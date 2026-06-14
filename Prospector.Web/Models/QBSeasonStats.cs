namespace Prospector.Web.Models;

public class QBSeasonStats
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public int Year { get; set; }
    public string Season { get; set; } = "Regular";

    // Volume stats
    public int GamesPlayed { get; set; }
    public int GamesStarted { get; set; }
    public int Attempts { get; set; }
    public int Completions { get; set; }
    public int PassingYards { get; set; }
    public int PassingTDs { get; set; }
    public int Interceptions { get; set; }
    public int Sacks { get; set; }
    public int SackYardsLost { get; set; }

    // Rushing
    public int RushingAttempts { get; set; }
    public int RushingYards { get; set; }
    public int RushingTDs { get; set; }

    // Efficiency (stored as computed/imported values)
    public double CompletionPct { get; set; }
    public double YardsPerAttempt { get; set; }
    public double AdjYardsPerAttempt { get; set; }
    public double? PasserRating { get; set; }

    // Advanced metrics
    public double? Qbr { get; set; }
    public double? Epa { get; set; }
    public double? EpaPerPlay { get; set; }
    public double? Cpoe { get; set; }
    public double? AirYardsPerAttempt { get; set; }
    public double? OnTargetThrowPct { get; set; }
    public double? PressureRate { get; set; }
    public double? SackRate { get; set; }
    public double? ThirdDownConvPct { get; set; }
    public double? RedZoneCompPct { get; set; }
    public double? DropPct { get; set; }
}
