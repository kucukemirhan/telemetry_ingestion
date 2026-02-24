using telemetry_ingestion.Interfaces;

namespace telemetry_ingestion.Parsers;

public class SpeedParser : ITelemetryParser
{
    public byte MessageType => 2;

    public string Parse(byte[] payload)
    {
        int speed = payload[0];
        bool running = (payload[1] & 0b00000001) != 0;
        return $"Speed:{speed} km/h | Running:{running}";
    }
}