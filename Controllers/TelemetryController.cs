using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Data;
using telemetry_ingestion.Models;
using telemetry_ingestion.Services;
//using telemetry_ingestion.Data;

namespace telemetry_ingestion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly TelemetryService _service;
        private readonly AppDbContext _context;

        public TelemetryController(TelemetryService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }
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

        [HttpPost]
        public async Task<IActionResult> Process([FromBody] string hexFrame)
        {
            try
            {
                var result = await _service.ProcessAsync(hexFrame, HttpContext.RequestAborted);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTelemetry(
        [FromQuery] int? deviceId,
        [FromQuery] string? sensorType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
        {
            var baseQuery = _context.Set<TelemetryRecordBase>().AsNoTracking().AsQueryable();

            // filters
            if (deviceId.HasValue)
                baseQuery = baseQuery.Where(t => t.DeviceId == deviceId.Value);

            if (from.HasValue)
                baseQuery = baseQuery.Where(t => t.Timestamp >= from.Value);

            if (to.HasValue)
                baseQuery = baseQuery.Where(t => t.Timestamp <= to.Value);

            // query from database
            var entities = await baseQuery.ToListAsync();

            // DTO mapping
            var telemetry = entities.Select(t => new TelemetryResponseDto
            {
                Id = t.Id,
                DeviceId = t.DeviceId,
                Timestamp = t.Timestamp,
                SensorType = t switch
                {
                    SpeedRecord => "Speed",
                    TemperatureRecord => "Temperature",
                    VibrationRecord => "Vibration",
                    _ => "Unknown"
                },
                Speed = t is SpeedRecord s ? s.Speed : null,
                Running = t is SpeedRecord s2 ? s2.Running : null,
                Temperature = t is TemperatureRecord temp ? temp.Temperature : null,
                Status = t is TemperatureRecord temp2 ? temp2.Status : null,
                Amplitude = t is VibrationRecord v ? v.Amplitude : null,
                Frequency = t is VibrationRecord v2 ? v2.Frequency : null
            }).ToList();

            if (!string.IsNullOrEmpty(sensorType))
                telemetry = telemetry
                    .Where(t => t.SensorType.Equals(sensorType, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return Ok(telemetry);
        }
    }
}