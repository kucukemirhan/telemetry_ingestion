using Microsoft.AspNetCore.Mvc;

namespace telemetry_ingestion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController
    {
        private readonly TelemetryService _service;
        public TelemetryController(TelemetryService service)
        {
            _service = service;
        }
        public async Task<string> Process(string hexFrame)
        {
            return await _service.ProcessAsync(hexFrame, CancellationToken.None);
        }
        public string Ping()
        {
            return "PONG";
        }
    }
}