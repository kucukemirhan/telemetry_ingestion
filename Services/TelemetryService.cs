using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Services;

public class TelemetryService
{
    private readonly Dictionary<byte, ITelemetryParser> _parsers;
    private readonly ITelemetryRepository _repository;
    private readonly DevicesService _deviceService;

    public TelemetryService(IEnumerable<ITelemetryParser> parsers, ITelemetryRepository repository, DevicesService deviceService)
    {
        _parsers = parsers.ToDictionary(p => p.MessageType);
        _repository = repository;
        _deviceService = deviceService;
    }

    public async Task<string> ProcessAsync(int deviceId, string hexFrame, CancellationToken ct)
    {
        byte[] frame = Convert.FromHexString(hexFrame);

        if (frame.Length < 3) throw new ArgumentException("Payload too short");

        byte messageType = frame[0];
        byte[] payload = frame.Skip(1).ToArray();

        if (!_parsers.TryGetValue(messageType, out var parser))
            throw new ArgumentException($"Unknown message type {messageType}");

        TelemetryRecordBase record = parser.Parse(payload);
        record.DeviceId = deviceId;
        record.MessageType = messageType;
        record.Timestamp = DateTime.UtcNow;
        record.RawPayload = payload;

        if (!await _deviceService.DeviceExistsAsync(deviceId, ct))
            throw new ArgumentException($"Device {deviceId} not found.");

        await _repository.AddAsync(record, ct);

        return $"Device:{deviceId} | Type:{messageType} | Saved as {record.GetType().Name}";
    }

    public Task<List<TelemetryRecordBase>> GetAsync(int? deviceId = null, string? sensorType = null, CancellationToken ct = default)
    {
        return _repository.GetAsync(deviceId, sensorType, ct);
    }
}