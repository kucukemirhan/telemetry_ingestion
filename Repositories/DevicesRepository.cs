using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Data;
using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Repositories;

public class DevicesRepository : IDevicesRepository
{
    private readonly AppDbContext _context;

    public DevicesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Device device, CancellationToken ct = default)
    {
        _context.Devices.Add(device);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<Device>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Devices.AsNoTracking().ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(int deviceId, CancellationToken ct = default)
    {
        return await _context.Devices.AnyAsync(d => d.Id == deviceId, ct);
    }
}