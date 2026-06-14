using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prospector.Web.Models;

namespace Prospector.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<QBSeasonStats> QBSeasonStats => Set<QBSeasonStats>();
    public DbSet<ScoutReport> ScoutReports => Set<ScoutReport>();
    public DbSet<QBPhysicalMeasurements> QBPhysicalMeasurements => Set<QBPhysicalMeasurements>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Player>(e =>
        {
            e.HasIndex(p => p.DraftClassYear);
            e.HasIndex(p => p.School);
            e.Property(p => p.OverallGrade).HasPrecision(5, 2);
            e.Property(p => p.CeilingGrade).HasPrecision(5, 2);
            e.Property(p => p.FloorGrade).HasPrecision(5, 2);
        });

        builder.Entity<QBSeasonStats>(e =>
        {
            e.HasOne(s => s.Player)
             .WithMany(p => p.SeasonStats)
             .HasForeignKey(s => s.PlayerId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => new { s.PlayerId, s.Year, s.Season }).IsUnique();
        });

        builder.Entity<ScoutReport>(e =>
        {
            e.HasOne(r => r.Player)
             .WithMany(p => p.ScoutReports)
             .HasForeignKey(r => r.PlayerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<QBPhysicalMeasurements>(e =>
        {
            e.HasOne(m => m.Player)
             .WithMany(p => p.Measurements)
             .HasForeignKey(m => m.PlayerId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
