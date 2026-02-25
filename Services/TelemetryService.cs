using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Data;
using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Models;

namespace telemetry_ingestion.Services
{
    public class TelemetryService
    {
        private readonly Dictionary<byte, ITelemetryParser> _parsers;
        private readonly AppDbContext _context;

        public TelemetryService(IEnumerable<ITelemetryParser> parsers, AppDbContext context)
        {
            Console.WriteLine("TelemetryService created");
            _parsers = parsers.ToDictionary(p => p.MessageType);
            _context = context;
        }

        public async Task<string> ProcessAsync(string hexFrame, CancellationToken ct)
        {
            try
            {
                await Task.Delay(200, ct); // simulate latency

                byte[] frame = Convert.FromHexString(hexFrame);

                // Frame validation
                if (frame.Length < 4) {
                    throw new ArgumentException("Frame too short");
                }

                byte deviceId = frame[1];
                byte messageType = frame[2];
                byte payloadLength = frame[3];

                if (frame.Length < 4 + payloadLength) {
                    throw new ArgumentException($"Payload length mismatch for device {deviceId}");
                }

                byte[] payload = frame.Skip(4)
                                      .Take(payloadLength)
                                      .ToArray();

                if (!_parsers.TryGetValue(messageType, out var parser)) {
                    throw new ArgumentException($"Unknown message type {messageType}");
                }

                TelemetryRecordBase record = parser.Parse(payload);

                var deviceExists = await _context.Devices
                    .AnyAsync(d => d.Id == deviceId, ct);

                if (!deviceExists) {
                    throw new ArgumentException($"Device {deviceId} not found.");
                }

                record.DeviceId = deviceId;
                record.MessageType = messageType;
                record.RawPayload = payload;
                record.Timestamp = DateTime.UtcNow;

                _context.Add(record);

                await _context.SaveChangesAsync(ct);

                return $"Device:{deviceId} | Type:{messageType} | Saved as {record.GetType().Name}";
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
