using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<TelemetryRecord> TelemetryRecords => Set<TelemetryRecord>();
}