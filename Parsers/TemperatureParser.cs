using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Parsers;
public class TemperatureParser : ITelemetryParser
{
    public byte MessageType => 1;

    public TelemetryRecordBase Parse(byte[] payload)
    {
        return new TemperatureRecord
        {
            Temperature = payload[0],
            Status = payload[1] == 1
        };
    }
}