namespace telemetry_ingestion.Models;

public abstract class TelemetryRecordBase
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public DateTime Timestamp { get; set; }
    public byte[] RawPayload { get; set; } = Array.Empty<byte>();
    public byte MessageType { get; set; }

    public Device? Device { get; set; }
}
public class TemperatureRecord : TelemetryRecordBase
{
    public int Temperature { get; set; }
    public bool Status { get; set; }
}

public class SpeedRecord : TelemetryRecordBase
{
    public int Speed { get; set; }
    public bool Running { get; set; }
}

public class VibrationRecord : TelemetryRecordBase
{
    public int Amplitude { get; set; }
    public int Frequency { get; set; }
}