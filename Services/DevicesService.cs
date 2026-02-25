using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Services;

public class DevicesService
{
    private readonly IDevicesRepository _repository;

    public DevicesService(IDevicesRepository repository)
    {
        _repository = repository;
    }

    public Task AddDeviceAsync(Device device, CancellationToken ct = default)
    {
        return _repository.AddAsync(device, ct);
    }

    public Task<List<Device>> GetAllDevicesAsync(CancellationToken ct = default)
    {
        return _repository.GetAllAsync(ct);
    }

    public Task<bool> DeviceExistsAsync(int deviceId, CancellationToken ct = default)
    {
        return _repository.ExistsAsync(deviceId, ct);
    }
}