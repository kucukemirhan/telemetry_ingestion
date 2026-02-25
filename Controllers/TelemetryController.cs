using Microsoft.AspNetCore.Mvc;
using telemetry_ingestion.DTOs;
using telemetry_ingestion.Models;
using telemetry_ingestion.Services;

namespace telemetry_ingestion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly TelemetryService _service;

        public TelemetryController(TelemetryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Process([FromBody] TelemetryRequestDto dto, CancellationToken ct)
        {
            var result = await _service.ProcessAsync(dto.DeviceId, dto.RawPayload, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTelemetry([FromQuery] int? deviceId, [FromQuery] string? sensorType, CancellationToken ct)
        {
            var entities = await _service.GetAsync(deviceId, sensorType, ct);

            var telemetry = entities.Select(t =>
            {
                var dto = new TelemetryResponseDto
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
                    }
                };

                if (t is SpeedRecord s) { dto.Speed = s.Speed; dto.Running = s.Running; }
                else if (t is TemperatureRecord temp) { dto.Temperature = temp.Temperature; dto.Status = temp.Status; }
                else if (t is VibrationRecord v) { dto.Amplitude = v.Amplitude; dto.Frequency = v.Frequency; }

                return dto;
            }).ToList();

            return Ok(telemetry);
        }
    }
}