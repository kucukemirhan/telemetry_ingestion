using Microsoft.EntityFrameworkCore;
using telemetry_ingestion.Services;
using telemetry_ingestion.Interfaces;
using telemetry_ingestion.Parsers;
using telemetry_ingestion.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ITelemetryParser, TemperatureParser>();
builder.Services.AddScoped<ITelemetryParser, SpeedParser>();
builder.Services.AddScoped<ITelemetryParser, VibrationParser>();
builder.Services.AddScoped<TelemetryService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

// swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();