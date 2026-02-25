using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<TelemetryRecord> TelemetryRecords => Set<TelemetryRecord>();
    public DbSet<Device> Devices => Set<Device>();

    // override to prevent EF to use convention-based mapping
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelemetryRecord>()
            .HasOne(t => t.Device)                  // TelemetryRecord has one Device
            .WithMany(d => d.TelemetryRecords)     // Device has many TelemetryRecords
            .HasForeignKey(t => t.DeviceId)        // FK column
            .OnDelete(DeleteBehavior.Cascade);     // Delete behavior
    }
}

