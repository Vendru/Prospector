namespace Prospector.Web.Models;

public class ScoutReport
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public string Organization { get; set; } = "Prospector Analytics";
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;

    // Skill grades (0–100)
    public double ArmStrength { get; set; }
    public double Accuracy { get; set; }
    public double DeepBallAccuracy { get; set; }
    public double Footwork { get; set; }
    public double Mechanics { get; set; }
    public double DecisionMaking { get; set; }
    public double PocketPresence { get; set; }
    public double MobilityAgility { get; set; }
    public double Leadership { get; set; }
    public double FootballIq { get; set; }
    public double ReleaseSpeed { get; set; }

    // Summary
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? NflComparison { get; set; }
}
