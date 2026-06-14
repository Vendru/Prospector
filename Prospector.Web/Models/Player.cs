namespace Prospector.Web.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string Conference { get; set; } = string.Empty;
    public string Position { get; set; } = "QB";
    public int DraftClassYear { get; set; }
    public int HeightInches { get; set; }
    public int WeightLbs { get; set; }
    public string HomeTown { get; set; } = string.Empty;
    public string HomeState { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? NflComparison { get; set; }
    public int? JerseyNumber { get; set; }

    // Draft projection
    public int? ProjectedRound { get; set; }
    public int? ProjectedPick { get; set; }
    public double? OverallGrade { get; set; }
    public double? CeilingGrade { get; set; }
    public double? FloorGrade { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QBSeasonStats> SeasonStats { get; set; } = [];
    public ICollection<ScoutReport> ScoutReports { get; set; } = [];
    public ICollection<QBPhysicalMeasurements> Measurements { get; set; } = [];
}
