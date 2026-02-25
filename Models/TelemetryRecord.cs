namespace telemetry_ingestion.Models;

public class TelemetryRecord
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public Device? Device { get; set; } // Navigation property
    public byte MessageType { get; set; }
    public string ParsedData { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}