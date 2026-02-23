using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ITelemetryParser, TemperatureParser>();
builder.Services.AddScoped<ITelemetryParser, SpeedParser>();
builder.Services.AddScoped<ITelemetryParser, VibrationParser>();
builder.Services.AddScoped<TelemetryService>();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

// swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();

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
        Console.WriteLine("TelemetryService created");
        _parsers = parsers.ToDictionary(p => p.MessageType);
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

            return $"Device:{deviceId} | Type:{messageType} | {parsed}";
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

#endregion