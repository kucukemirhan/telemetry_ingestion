using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Parsers;

public class SpeedParser : ITelemetryParser
{
    public byte MessageType => 2;

    public TelemetryRecordBase Parse(byte[] payload)
    {
        return new SpeedRecord
        {
            Speed = payload[0],
            Running = (payload[1] & 0b00000001) != 0
        };
    }
}