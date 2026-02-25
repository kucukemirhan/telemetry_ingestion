using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<TemperatureRecord> TemperatureRecords => Set<TemperatureRecord>();
    public DbSet<SpeedRecord> SpeedRecords => Set<SpeedRecord>();
    public DbSet<VibrationRecord> VibrationRecords => Set<VibrationRecord>();

    // override to prevent EF to use convention-based mapping
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelemetryRecordBase>()
            .HasOne(t => t.Device)
            .WithMany(d => d.TelemetryRecords)
            .HasForeignKey(t => t.DeviceId);

        modelBuilder.Entity<TemperatureRecord>().ToTable("TemperatureRecords");
        modelBuilder.Entity<SpeedRecord>().ToTable("SpeedRecords");
        modelBuilder.Entity<VibrationRecord>().ToTable("VibrationRecords");
    }
}