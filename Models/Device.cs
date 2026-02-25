namespace telemetry_ingestion.Models;

public class Device
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string ProtocolType { get; set; } = string.Empty;

    // Navigation property
    public List<TelemetryRecordBase> TelemetryRecords { get; set; } = new();
}