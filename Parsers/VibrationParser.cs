using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Parsers;

public class VibrationParser : ITelemetryParser
{
    public byte MessageType => 3;

    public TelemetryRecordBase Parse(byte[] payload)
    {
        return new VibrationRecord
        {
            Amplitude = payload[0],
            Frequency = payload[1]
        };
    }
}