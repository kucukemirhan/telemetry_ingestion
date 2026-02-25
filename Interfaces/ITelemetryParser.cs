using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Interfaces
{
    public interface ITelemetryParser
    {
        byte MessageType { get; }
        TelemetryRecordBase Parse(byte[] payload);
    }
}
