# Telemetry Ingestion API

Industrial-style telemetry ingestion backend built with:

- ASP.NET Core (.NET 8)
- Entity Framework Core
- PostgreSQL
- Docker

## Purpose

This project receives a hex telemetry frame via HTTP, parses it, and persists it into PostgreSQL.

System flow: Machine -> API -> Parse -> PostgreSQL (Saved)

 
## Architecture

Controller -> TelemetryService -> AppDbContext (EF Core) -> PostgreSQL (Docker Container)

- Controller: The entry point. Receives incoming HTTP requests and routes the raw payload.
- TelemetryService: The brain. Parses the raw hexadecimal string into a structured data object.
- AppDbContext (EF Core): The bridge. Translates C# objects into SQL commands (ORM).
- PostgreSQL (Docker): The vault. Securely persists the processed data in an isolated container.

---

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

### Entity Model
This is the blueprint of our data. Entity Framework uses this C# class to automatically design the relational database schema.

```powershell
public class TelemetryRecord
{
    public int Id { get; set; } // Auto increment primary key
    public byte DeviceId { get; set; }
    public byte MessageType { get; set; }
    public string ParsedData { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
```
(Think of it as: Class = Table, Property = Column)

## Migration 
Time to sync our code with the database. These commands generate the SQL instructions and apply them to construct our tables.

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

Open the Swagger UI to interact with the API::
```code
http://localhost:5000/swagger/index.html
```

Send a POST request -> /api/Telemetry

Example body:
```json
"AA0101FFFF"
```
After sending the body,

Verify data in database:
```powershell
docker exec -it telemetry-db psql -U postgres -d telemetrydb
```
Query the records:
```powershell
SELECT * FROM "TelemetryRecords";
```

If records are visible, the ingestion pipeline is working correctly.

Tip: Type \dt inside the psql shell to list all tables in your database.