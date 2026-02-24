using telemetry_ingestion.Interfaces;

namespace telemetry_ingestion.Parsers;

public class TemperatureParser : ITelemetryParser
{
    public byte MessageType => 1;

    public string Parse(byte[] payload)
    {
        int temp = payload[0];
        bool status = payload[1] == 1;
        return $"Temperature: {temp}°C | Status:{status}";
    }
}