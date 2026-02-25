using telemetry_ingestion.Models;

namespace telemetry_ingestion.Interfaces
{
    public interface IDevicesRepository
    {
        Task AddAsync(Device device, CancellationToken ct = default);
        Task<List<Device>> GetAllAsync(CancellationToken ct = default);
        Task<bool> ExistsAsync(int deviceId, CancellationToken ct = default);
    }
}
