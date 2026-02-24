using telemetry_ingestion.Interfaces;

namespace telemetry_ingestion.Parsers;

public class VibrationParser : ITelemetryParser
{
    public byte MessageType => 3;

    public string Parse(byte[] payload)
    {
        int amplitude = payload[0];
        int frequency = payload[1];
        return $"Vibration Amp:{amplitude} | Freq:{frequency}";
    }
}