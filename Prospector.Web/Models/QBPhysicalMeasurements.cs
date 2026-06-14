namespace Prospector.Web.Models;

public class QBPhysicalMeasurements
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public string Event { get; set; } = "Combine";
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;

    public double? FortyYardDash { get; set; }
    public double? TwentyYardSplit { get; set; }
    public double? TenYardSplit { get; set; }
    public double? VerticalJump { get; set; }
    public double? BroadJump { get; set; }
    public double? ThreeConeDrill { get; set; }
    public double? ShuttleRun { get; set; }
    public double? HandSizeInches { get; set; }
    public double? ArmLengthInches { get; set; }
    public double? WingSpanInches { get; set; }
    public int? BenchReps { get; set; }
}
