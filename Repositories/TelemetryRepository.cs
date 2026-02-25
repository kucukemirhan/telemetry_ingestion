using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Data;
using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly AppDbContext _context;

    public TelemetryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TelemetryRecordBase record, CancellationToken ct)
    {
        _context.Add(record);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<TelemetryRecordBase>> GetAsync(int? deviceId = null, string? sensorType = null, CancellationToken ct = default)
    {
        var query = _context.Set<TelemetryRecordBase>().AsNoTracking().AsQueryable();

        if (deviceId.HasValue)
            query = query.Where(t => t.DeviceId == deviceId.Value);

        var entities = await query.ToListAsync(ct);

        if (!string.IsNullOrEmpty(sensorType))
        {
            entities = entities.Where(t =>
                t switch
                {
                    SpeedRecord => sensorType.Equals("Speed", StringComparison.OrdinalIgnoreCase),
                    TemperatureRecord => sensorType.Equals("Temperature", StringComparison.OrdinalIgnoreCase),
                    VibrationRecord => sensorType.Equals("Vibration", StringComparison.OrdinalIgnoreCase),
                    _ => false
                }).ToList();
        }

        return entities;
    }
}