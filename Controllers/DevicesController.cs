using Microsoft.AspNetCore.Mvc;
using telemetry_ingestion.Models;
using telemetry_ingestion.Services;

namespace telemetry_ingestion.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly DevicesService _service;

    public DevicesController(DevicesService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Device device, CancellationToken ct)
    {
        await _service.AddDeviceAsync(device, ct);
        return Ok(device);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var devices = await _service.GetAllDevicesAsync(ct);
        return Ok(devices);
    }
}