using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telemetry_ingestion.DTOs
{
    public class TelemetryResponseDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public string SensorType { get; set; } = string.Empty;
        public int? Speed { get; set; }
        public bool? Running { get; set; }
        public int? Temperature { get; set; }
        public bool? Status { get; set; }
        public int? Amplitude { get; set; }
        public int? Frequency { get; set; }
    }
}
