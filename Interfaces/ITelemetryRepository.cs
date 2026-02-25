using telemetry_ingestion.Models;

namespace telemetry_ingestion.Interfaces
{
    public interface ITelemetryRepository
    {
        Task AddAsync(TelemetryRecordBase record, CancellationToken ct);
        Task<List<TelemetryRecordBase>> GetAsync(int? deviceId = null, string? sensorType = null, CancellationToken ct = default);
    }
}
