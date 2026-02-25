# Telemetry Ingestion API

Industrial-style telemetry ingestion backend built with:

- ASP.NET Core (.NET 8)
- Entity Framework Core
- PostgreSQL
- Docker

## Purpose

Industrial devices send raw hexadecimal telemetry frames. 
This project receives a hex telemetry frame via HTTP, parses it, and persists it into PostgreSQL.

System flow: Machine -> API -> Parse -> Service -> Repository -> PostgreSQL

 
## Architecture

High Level Flow:

Client -> [Middleware] -> Controller -> Services -> Repositories -> EF Core -> PostgreSQL -> (Docker Container)

- Middleware: Its main objective is Exception handling.
- Controller: HTTP entry point. Accepts request DTO and returns proper HTTP status codes.
- Services: 
    - TelemetryService: Parses and validates hex frame.
    - DevicesService: Manages devices (add, list, check existence)
- Repositories: Abstract database access, separate repositories for devices and telemetry.
- Entity Framework (EF Core): Translates C# objects into SQL commands (ORM).
- PostgreSQL (Docker): Stores structured telemetry data in an isolated container.

## Entity Model

This is the blueprint of our data. Entity Framework uses this C# class to automatically design the relational database schema.
(Think of it as: Class = Table, Property = Column)

This class represents a physical industrial device:
```csharp
public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ProtocolType { get; set; } = string.Empty;

    public ICollection<TelemetryRecord> TelemetryRecords { get; set; }
        = new List<TelemetryRecord>();
}
```

And this one has the base properties of a single telemetry event:

```csharp
public class TelemetryRecordBase
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public DateTime Timestamp { get; set; }
    public byte[] RawPayload { get; set; } = Array.Empty<byte>();
    public byte MessageType { get; set; }

    public Device? Device { get; set; }
}
```
Every sensor type has its own DB schema, for example SpeedRecord:
```csharp
public class SpeedRecord : TelemetryRecordBase
{
    public int Speed { get; set; }
    public bool Running { get; set; }
}
```

EF model has Database-level constraint:

```
FOREIGN KEY (DeviceId) REFERENCES Devices(Id)
```

Meaning:

- A telemetry record MUST belong to an existing device.
- You cannot insert telemetry for a non-registered device.
- PostgreSQL enforces referential integrity.

This ensures:

Device (1) → (Many) TelemetryRecords

## Docker & PostgreSQL Setup

Start PostgreSQL container:

```powershell
docker run --name telemetry-db `
-e POSTGRES_USER=postgres `
-e POSTGRES_PASSWORD=StrongPass123! `
-e POSTGRES_DB=telemetrydb `
-p 5432:5432 `
-d postgres:16
```
This command creates and starts a fresh PostgreSQL 16 container ready to accept connections.


## Entity Framework Setup
Next, we equip our .NET project with the necessary Object-Relational Mapping (ORM) tools and the PostgreSQL provider.

Install required packages:

```powershell
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.5
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.5
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.5
```

Install EF CLI Tool (for running migrations):

```powershell
dotnet tool install --global dotnet-ef
```


### Migration 
This is the process of syncing the code with the database. These commands generate the SQL instructions and apply them to construct our tables.

Generate the initial migration:

```powershell
dotnet ef migrations add InitialCreate
```
Apply migration to the database:

```powershell
dotnet ef database update
```


## Run & Test

Start application:

```powershell
dotnet run
```

Open the Swagger UI to interact with the API:
```code
http://localhost:5000/swagger/index.html
```

#### Register a Device:

POST /api/devices

```json
{
  "name": "Pump A",
  "location": "Factory Floor",
  "protocolType": "Modbus"
}
```
#### Send Telemetry:

POST /api/telemetry

```json
{
  "deviceId": 1,
  "rawPayload": "020132"
}
```

---

#### Query Telemetry:

GET /api/telemetry

Optional filters:
```
/api/telemetry?deviceId=1
/api/telemetry?sensorType=Speed
```

Returns structured telemetry response DTO (Data transfer object).

### Verify Database:

Connect to container:

```powershell
docker exec -it telemetry-db psql -U postgres -d telemetrydb
```

List tables:

```
\dt
```

You should see all the tables. (Devices, TelemetryRecordBase,TemperatureRecords, VibrationRecords and SpeedRecords)