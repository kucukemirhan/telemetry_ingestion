using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#region Interface

public interface ITelemetryParser
{
    byte MessageType { get; }
    string Parse(byte[] payload);
}

#endregion

#region Parsers

public class TemperatureParser : ITelemetryParser
{
    public byte MessageType => 1;

    public string Parse(byte[] payload)
    {
        int temp = payload[0];
        bool status = payload[1] == 1;
        return $"Temperature: {temp}°C | Status:{status}";
    }
}

public class SpeedParser : ITelemetryParser
{
    public byte MessageType => 2;

    public string Parse(byte[] payload)
    {
        int speed = payload[0];
        bool running = (payload[1] & 0b00000001) != 0;
        return $"Speed:{speed} km/h | Running:{running}";
    }
}

public class VibrationParser : ITelemetryParser
{
    public byte MessageType => 3;

    public string Parse(byte[] payload)
    {
        int amplitude = payload[0];
        int frequency = payload[1];
        return $"Vibration Amp:{amplitude} | Freq:{frequency}";
    }
}

#endregion

#region Service

public class TelemetryService
{
    private readonly Dictionary<byte, ITelemetryParser> _parsers;

    public TelemetryService(IEnumerable<ITelemetryParser> parsers)
    {
        _parsers = parsers.ToDictionary(p => p.MessageType);
    }

    public async Task ProcessAsync(string hexFrame, CancellationToken ct)
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
                Console.WriteLine($"Invalid HEX frame: {hexFrame}");
                return;
            }

            // Frame validation
            if (frame.Length < 4)
            {
                Console.WriteLine("Frame too short");
                return;
            }

            byte deviceId = frame[1];
            byte messageType = frame[2];
            byte payloadLength = frame[3];

            if (frame.Length < 4 + payloadLength)
            {
                Console.WriteLine($"Payload length mismatch for device {deviceId}");
                return;
            }

            byte[] payload = frame.Skip(4)
                                  .Take(payloadLength)
                                  .ToArray();

            if (!_parsers.TryGetValue(messageType, out var parser))
            {
                Console.WriteLine($"Unknown message type {messageType}");
                return;
            }

            string parsed = parser.Parse(payload);

            Console.WriteLine(
                $"Device:{deviceId} | Type:{messageType} | {parsed}"
            );
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Processing cancelled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}

#endregion

#region Frame Generator

public static class IndustrialFrameGenerator
{
    public static string GenerateFrame(int deviceId)
    {
        Random rnd = Random.Shared;

        byte start = 0xAA;
        byte messageType = (byte)rnd.Next(1, 4);

        byte[] payload = messageType switch
        {
            1 => new byte[]
            {
                (byte)rnd.Next(20, 80),
                (byte)rnd.Next(0, 2)
            },
            2 => new byte[]
            {
                (byte)rnd.Next(0, 150),
                (byte)rnd.Next(0, 2)
            },
            3 => new byte[]
            {
                (byte)rnd.Next(0, 100),
                (byte)rnd.Next(0, 200)
            },
            _ => Array.Empty<byte>()
        };

        byte payloadLength = (byte)payload.Length;

        var frame = new List<byte>
        {
            start,
            (byte)deviceId,
            messageType,
            payloadLength
        };

        frame.AddRange(payload);

        return Convert.ToHexString(frame.ToArray());
    }
}

#endregion

#region Program

class Program
{
    static async Task Main()
    {
        // Dependency creation
        var parsers = new List<ITelemetryParser>
        {
            new TemperatureParser(),
            new SpeedParser(),
            new VibrationParser()
        };

        var service = new TelemetryService(parsers);

        // Cancellation control
        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Stopping...");
            cts.Cancel();
            e.Cancel = true;
        };

        // Device simulation
        var tasks = new List<Task>();

        for (int deviceId = 1; deviceId <= 5; deviceId++)
        {
            int capturedId = deviceId;

            tasks.Add(Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    string frame = IndustrialFrameGenerator.GenerateFrame(capturedId);

                    await service.ProcessAsync(frame, cts.Token);

                    await Task.Delay(1000, cts.Token);
                }
            }));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Application shutting down...");
        }
    }
}

#endregion