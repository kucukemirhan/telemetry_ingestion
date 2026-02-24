namespace telemetry_ingestion.Models;

public class TelemetryRecord
{
    public int Id { get; set; }
    public byte DeviceId { get; set; }
    public byte MessageType { get; set; }
    public string ParsedData { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}