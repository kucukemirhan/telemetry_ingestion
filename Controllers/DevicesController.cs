using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Data;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DevicesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Device device)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return Ok(device);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _context.Devices.ToListAsync();
        return Ok(devices);
    }
}