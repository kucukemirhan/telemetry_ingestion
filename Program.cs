using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Data;
using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Middlewares;
using telemetry_ingestion.Parsers;
using telemetry_ingestion.Repositories;
using telemetry_ingestion.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IDevicesRepository, DevicesRepository>();
builder.Services.AddScoped<DevicesService>();

builder.Services.AddScoped<ITelemetryRepository, TelemetryRepository>();
builder.Services.AddScoped<TelemetryService>();

builder.Services.AddScoped<ITelemetryParser, TemperatureParser>();
builder.Services.AddScoped<ITelemetryParser, SpeedParser>();
builder.Services.AddScoped<ITelemetryParser, VibrationParser>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

// swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();