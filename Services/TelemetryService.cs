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

                byte[] frame;

                try
                {
                    frame = Convert.FromHexString(hexFrame);
                }
                catch
                {
                    throw;
                }

                // Frame validation
                if (frame.Length < 4)
                {
                    throw new ArgumentException("Frame too short");
                }

                byte deviceId = frame[1];
                byte messageType = frame[2];
                byte payloadLength = frame[3];

                if (frame.Length < 4 + payloadLength)
                {
                    throw new ArgumentException($"Payload length mismatch for device {deviceId}");
                }

                byte[] payload = frame.Skip(4)
                                      .Take(payloadLength)
                                      .ToArray();

                if (!_parsers.TryGetValue(messageType, out var parser))
                {
                    throw new ArgumentException($"Unknown message type {messageType}");
                }

                string parsed = parser.Parse(payload);

                var record = new TelemetryRecord
                {
                    DeviceId = deviceId,
                    MessageType = messageType,
                    ParsedData = parsed,
                    Timestamp = DateTime.UtcNow
                };

                _context.TelemetryRecords.Add(record);
                await _context.SaveChangesAsync(ct);

                return $"Device:{deviceId} | Type:{messageType} | {parsed}";
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
