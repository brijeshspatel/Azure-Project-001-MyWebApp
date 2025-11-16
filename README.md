# MyWebApp

A clean architecture ASP.NET Core 8.0 Web API project demonstrating best practices for building modern web applications.

## Overview

MyWebApp is a weather forecast API built with a layered architecture approach, separating concerns into Core, Infrastructure, and API layers.

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

```
MyWebApp/
├── MyWebApp/                    # API Layer (Controllers, Program.cs)
├── MyWebApp.Core/              # Domain Layer (Models, Interfaces)
├── MyWebApp.Infrastructure/    # Infrastructure Layer (Services, Data Access)
└── MyWebApp.Tests.Unit/        # Unit Tests
```

### Project Structure

- **MyWebApp** - ASP.NET Core Web API with controllers and application configuration
- **MyWebApp.Core** - Domain models and service interfaces (no dependencies)
- **MyWebApp.Infrastructure** - Concrete implementations of services and data access
- **MyWebApp.Tests.Unit** - Unit tests for the application

## Technology Stack

- **.NET 8.0** - Latest LTS version of .NET
- **ASP.NET Core** - Web API framework
- **Swagger/OpenAPI** - API documentation and testing
- **C# 12.0** - Latest C# language features

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- IDE: Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Running the Application

1. Clone the repository
2. Navigate to the solution directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```
5. Run the application:
   ```bash
   dotnet run --project MyWebApp
   ```

The API will be available at `https://localhost:7xxx` (port may vary). Swagger UI will be accessible at `https://localhost:7xxx/swagger`.

## API Endpoints

### Weather Forecast

- **GET** `/weatherforecast` - Returns a 5-day weather forecast

### Query Parameters

- `days` (optional): Number of days for the forecast (1-30, default: 5)
- `location` (optional): Location name for the forecast (max 100 characters)

### Error Responses

All errors follow RFC 7807 Problem Details format:

- **400 Bad Request** - Validation errors or invalid input
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Unexpected server errors

Example error response:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/weatherforecast",
  "errors": {
    "Days": ["The number of days cannot exceed 30."]
  },
  "errorCode": "VAL000",
  "traceId": "0af7651916cd43dd8448eb211c80319c",
  "spanId": "b7ad6b7169203331"
}
```

### Distributed Tracing

The API implements W3C Trace Context standard for distributed tracing:

- **traceId**: Globally unique identifier (32-char hex) for the entire request across all services
- **spanId**: Unique identifier (16-char hex) for this specific operation
- **parentSpanId**: Identifier of the parent operation (for distributed systems)

These correlation IDs enable:
- End-to-end request tracking across microservices
- Performance monitoring and bottleneck identification
- Error diagnosis in distributed systems
- Integration with Azure Application Insights and OpenTelemetry

For more details, see [W3C Trace Context Implementation](.docs/W3C_TRACE_CONTEXT_IMPLEMENTATION.md).

## Development

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Code Style

The project uses `.editorconfig` for consistent code formatting. Configuration is centralized in `Directory.Build.props` for shared properties across all projects.

## Versioning

The solution uses centralized versioning through `Directory.Build.props`:
- Assembly versioning follows semantic versioning (SemVer)
- Version information is automatically included in all project assemblies

## Project Features

- ✅ Clean Architecture with clear layer separation
- ✅ Dependency Injection configured in Program.cs
- ✅ Swagger/OpenAPI documentation
- ✅ Centralized build configuration
- ✅ Unit test project setup
- ✅ EditorConfig for code consistency
- ✅ Global exception handling with RFC 7807 Problem Details
- ✅ FluentValidation for request validation
- ✅ Structured error responses
- ✅ W3C Trace Context distributed tracing
- ✅ Azure Application Insights ready
- ✅ OpenTelemetry compatible

## Contributing

This is a personal project for learning and demonstration purposes.

## License

[Specify your license here]

---

**Note**: This project is part of MyAzureProject001 and serves as a foundation for learning modern .NET development practices.
