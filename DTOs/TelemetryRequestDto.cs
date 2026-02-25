using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemetry_ingestion.DTOs
{
    public class TelemetryRequestDto
    {
        public int DeviceId { get; set; }
        public string RawPayload { get; set; } = string.Empty;
    }
}
