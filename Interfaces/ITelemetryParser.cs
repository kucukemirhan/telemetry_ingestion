using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemetry_ingestion.Interfaces
{
    public interface ITelemetryParser
    {
        byte MessageType { get; }
        string Parse(byte[] payload);
    }
}
