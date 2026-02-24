using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Process([FromBody] string hexFrame)
        {
            var result = await _service.ProcessAsync(hexFrame, HttpContext.RequestAborted);
            return Ok(result);
        }
    }
}