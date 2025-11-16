# MyWebApp

A production-ready ASP.NET Core 8.0 Web API project demonstrating clean architecture, comprehensive testing, and modern .NET development best practices.

## 🎯 Project Status

- ✅ **Phase 1**: Foundation & Clean Architecture - Complete
- ✅ **Phase 2**: Core Domain & Business Logic - Complete  
- ✅ **Phase 3**: Comprehensive Unit Testing - **Complete (205 tests passing!)**
- 📊 **Test Coverage**: >80% across all layers
- 🚀 **Production Ready**: Full exception handling, validation, and observability

## Overview

MyWebApp is a weather forecast API built with Clean Architecture principles, featuring:

- **Layered Architecture** with clear separation of concerns
- **Comprehensive Test Suite** with 205+ unit tests
- **Production-Grade Error Handling** with RFC 7807 Problem Details
- **Distributed Tracing** with W3C Trace Context
- **Request Validation** using FluentValidation
- **Azure-Ready** with Application Insights integration

## Architecture

The solution follows Clean Architecture principles with clear separation of concerns:

```
MyWebApp/
├── MyWebApp/                    # API Layer (Controllers, Middleware, Program.cs)
├── MyWebApp.Core/              # Domain Layer (Models, Interfaces, Validators)
├── MyWebApp.Infrastructure/    # Infrastructure Layer (Services, Implementations)
└── MyWebApp.Tests.Unit/        # Comprehensive Unit Tests (205 tests)
```

### Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|---------------|--------------|
| **MyWebApp** | API endpoints, middleware, configuration | Core, Infrastructure |
| **MyWebApp.Core** | Domain models, business rules, interfaces | None |
| **MyWebApp.Infrastructure** | Service implementations, data access | Core |
| **MyWebApp.Tests.Unit** | Unit tests for all layers | All projects |

### Key Components

#### Middleware
- **GlobalExceptionHandler** - Centralised exception handling with W3C Trace Context correlation

#### Validation
- **FluentValidation** - Request validation with detailed error messages and error codes
- **GetWeatherForecastRequestValidator** - Business rule validation for forecast requests

#### Services
- **WeatherForecastService** - Weather forecast business logic with validation integration

#### Exception Hierarchy
```
Exception
└── DomainException (WF base)
    ├── ValidationException (VAL errors)
    ├── BusinessRuleException (BUS errors)
    ├── NotFoundException (NF errors)
    └── WeatherForecastException (WF errors)
```

## Technology Stack

### Core Technologies
- **.NET 8.0** - Latest LTS version of .NET
- **ASP.NET Core** - Web API framework with minimal APIs support
- **C# 12.0** - Latest language features (primary constructors, collection expressions)

### Libraries & Frameworks
- **FluentValidation 11.x** - Request validation
- **Swagger/OpenAPI** - API documentation and testing

### Testing Stack
- **xUnit 2.5.3** - Test framework
- **Moq 4.20.72** - Mocking framework
- **AutoFixture 4.18.1** - Test data generation
- **FluentAssertions 8.8.0** - Readable assertions
- **Coverlet** - Code coverage

### Observability
- **W3C Trace Context** - Distributed tracing standard
- **Azure Application Insights** - Ready for integration
- **OpenTelemetry** - Compatible tracing

## Getting Started

### Prerequisites

- **Required**:
  - [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
  
- **Recommended**:
  - Visual Studio 2022 (v17.8+) with ASP.NET workload
  - Visual Studio Code with C# extension
  - JetBrains Rider 2023.3+

- **Optional**:
  - Git for version control
  - Azure CLI for deployment
  - Docker Desktop for containerisation

### Quick Start

```bash
# 1. Clone the repository
git clone <repository-url>
cd Azure-Project-001-MyWebApp

# 2. Restore dependencies
dotnet restore

# 3. Build the solution
dotnet build

# 4. Run tests (optional but recommended)
dotnet test

# 5. Run the application
dotnet run --project MyWebApp
```

### Detailed Setup

#### 1. Clone and Navigate
```bash
git clone <repository-url>
cd Azure-Project-001-MyWebApp/Solution/Azure-Project-001-MyWebApp
```

#### 2. Verify .NET Installation
```bash
dotnet --version
# Should show 8.0.x or later
```

#### 3. Restore and Build
```bash
dotnet restore
dotnet build --configuration Release
```

#### 4. Run Tests
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage
dotnet test /p:CollectCoverage=true
```

#### 5. Run the Application
```bash
# Development mode (with Swagger)
dotnet run --project MyWebApp

# Production mode
dotnet run --project MyWebApp --configuration Release
```

### Accessing the API

Once running, the API is available at:

- **HTTPS**: `https://localhost:7xxx` (port assigned automatically)
- **HTTP**: `http://localhost:5xxx` (port assigned automatically)
- **Swagger UI**: `https://localhost:7xxx/swagger`

> **Note**: The actual port numbers are displayed in the console when the application starts.

## API Documentation

### Endpoints

#### Weather Forecast
```
GET /api/weatherforecast
```

Returns weather forecasts based on query parameters.

### Request Parameters

| Parameter | Type | Required | Default | Validation | Description |
|-----------|------|----------|---------|------------|-------------|
| `days` | integer | No | 5 | 1-30 | Number of forecast days |
| `location` | string | No | null | Max 100 chars | Location name |
| `includeDetails` | boolean | No | false | - | Include detailed information |

### Example Requests

#### Basic Request (Default 5 days)
```bash
curl -X GET "https://localhost:7xxx/api/weatherforecast"
```

#### Request with Parameters
```bash
curl -X GET "https://localhost:7xxx/api/weatherforecast?days=7&location=London"
```

#### Request with All Parameters
```bash
curl -X GET "https://localhost:7xxx/api/weatherforecast?days=10&location=New%20York&includeDetails=true"
```

### Response Format

#### Success Response (200 OK)
```json
[
  {
    "date": "2025-01-01",
    "temperatureC": 15,
    "temperatureF": 59,
    "summary": "Mild",
    "location": "London"
  },
  ...
]
```

### Error Responses

All errors follow **RFC 7807 Problem Details** format with W3C Trace Context correlation IDs.

#### 400 Bad Request - Validation Error
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The number of days cannot exceed 30.",
  "instance": "/api/weatherforecast",
  "errors": {
    "Days": ["The number of days cannot exceed 30."]
  },
  "errorCode": "WF_DAYS_TOO_HIGH",
  "traceId": "0af7651916cd43dd8448eb211c80319c",
  "spanId": "b7ad6b7169203331"
}
```

#### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Resource not found.",
  "status": 404,
  "detail": "The requested resource was not found.",
  "instance": "/api/weatherforecast/123",
  "errorCode": "NF001",
  "entityType": "WeatherForecast",
  "entityId": "123",
  "traceId": "a1b2c3d4e5f6...",
  "spanId": "1a2b3c4d..."
}
```

#### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "detail": "An unexpected error occurred. Please try again later.",
  "instance": "/api/weatherforecast",
  "traceId": "x1y2z3...",
  "spanId": "a1b2c3..."
}
```

### Error Codes

| Code | Category | Description |
|------|----------|-------------|
| `WF_DAYS_TOO_LOW` | Validation | Days parameter below minimum (1) |
| `WF_DAYS_TOO_HIGH` | Validation | Days parameter above maximum (30) |
| `WF_LOCATION_TOO_LONG` | Validation | Location exceeds 100 characters |
| `WF_LOCATION_INVALID` | Validation | Location contains invalid characters |
| `WF001` | Business | Invalid day range |
| `WF002` | Business | Forecast unavailable |
| `NF001` | NotFound | Entity not found |
| `VAL000` | Validation | General validation error |
| `VAL001` | Validation | Field validation error |
| `BUS000` | Business | Business rule violation |
| `DOM000` | Domain | General domain error |

### Distributed Tracing & Observability

The API implements **W3C Trace Context** standard for production-grade observability:

#### Trace Context Fields

| Field | Format | Description | Use Case |
|-------|--------|-------------|----------|
| `traceId` | 32-char hex | Globally unique trace identifier | End-to-end request tracking |
| `spanId` | 16-char hex | Operation-specific identifier | Component-level tracking |
| `parentSpanId` | 16-char hex | Parent operation identifier | Distributed call chains |

#### Benefits

- ✅ **End-to-end tracing** across microservices and distributed systems
- ✅ **Performance monitoring** and bottleneck identification
- ✅ **Error correlation** across service boundaries
- ✅ **Azure Application Insights** integration ready
- ✅ **OpenTelemetry** compatible
- ✅ **Production debugging** with correlation IDs

#### Integration Examples

**Azure Application Insights:**
```csharp
// Automatically tracked via Activity.Current
// TraceId and SpanId flow through the entire request pipeline
```

**Custom Logging:**
```csharp
_logger.LogInformation(
    "Processing request | TraceId: {TraceId} | SpanId: {SpanId}",
    Activity.Current?.TraceId.ToHexString(),
    Activity.Current?.SpanId.ToHexString()
);
```

## Development

### Building the Project

```bash
# Debug build (default)
dotnet build

# Release build
dotnet build --configuration Release

# Build specific project
dotnet build MyWebApp/MyWebApp.csproj

# Clean and rebuild
dotnet clean
dotnet build
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "FullyQualifiedName~WeatherForecastControllerTests"

# Run tests matching a pattern
dotnet test --filter "Name~Validation"

# List all tests without running
dotnet test --list-tests
```

### Test Suite Overview

The project includes **205 comprehensive unit tests** covering:

#### Test Categories

| Category | Tests | Coverage | Description |
|----------|-------|----------|-------------|
| **Exception Tests** | 99 | 100% | All custom exception classes |
| **Validator Tests** | 60 | 100% | FluentValidation rules |
| **Service Tests** | 22 | >85% | Business logic |
| **Controller Tests** | 24 | >80% | API endpoints |

#### Test Infrastructure

- **xUnit** - Test framework with theory-based data-driven tests
- **Moq** - Interface mocking for isolated unit tests
- **AutoFixture** - Automated test data generation
- **FluentAssertions** - Readable, expressive assertions

#### Example Test Patterns

**Theory with AutoMoqData:**
```csharp
[Theory, AutoMoqData]
public async Task GetForecastsAsync_ValidRequest_ReturnsForecasts(
    [Frozen] Mock<IValidator<GetWeatherForecastRequest>> mockValidator,
    WeatherForecastService sut,
    GetWeatherForecastRequest request)
{
    // Arrange
    mockValidator
        .Setup(v => v.ValidateAsync(request, default))
        .ReturnsAsync(new ValidationResult());
    
    // Act
    var result = await sut.GetForecastsAsync(request);
    
    // Assert
    result.Should().NotBeNull()
          .And.HaveCount(request.Days);
}
```

### Code Style & Standards

#### EditorConfig
The project uses `.editorconfig` for consistent formatting:
- 4-space indentation
- UTF-8 encoding
- LF line endings
- Trailing whitespace removal

#### Centralised Configuration
`Directory.Build.props` provides:
- Semantic versioning (Currently: **v1.2.3**)
- Assembly metadata
- Build configuration
- Code analysis rules

#### Naming Conventions
- **Classes**: PascalCase (e.g., `WeatherForecastService`)
- **Methods**: PascalCase (e.g., `GetForecastsAsync`)
- **Parameters**: camelCase (e.g., `cancellationToken`)
- **Private fields**: `_camelCase` with underscore prefix
- **Interfaces**: IPascalCase (e.g., `IWeatherForecastService`)

### Project Commands Reference

```bash
# Restore packages
dotnet restore

# Build
dotnet build [--configuration Release]

# Run
dotnet run --project MyWebApp [--configuration Release]

# Test
dotnet test [--verbosity normal] [--no-build]

# Clean
dotnet clean

# Format code
dotnet format

# List packages
dotnet list package

# Add package
dotnet add <project> package <PackageName>

# Update packages
dotnet list package --outdated
dotnet add <project> package <PackageName>
```

## Project Features

### ✅ Completed Features

#### Architecture & Design
- ✅ Clean Architecture with strict layer separation
- ✅ Dependency Injection with built-in DI container
- ✅ Repository pattern-ready infrastructure
- ✅ Domain-driven design principles

#### API & Documentation
- ✅ RESTful API design
- ✅ Swagger/OpenAPI documentation
- ✅ XML documentation comments
- ✅ Versioned API-ready

#### Validation & Error Handling
- ✅ FluentValidation with custom rules
- ✅ Global exception handling middleware
- ✅ RFC 7807 Problem Details responses
- ✅ Structured error codes and messages
- ✅ Custom exception hierarchy

#### Observability & Monitoring
- ✅ W3C Trace Context distributed tracing
- ✅ Azure Application Insights ready
- ✅ OpenTelemetry compatible
- ✅ Structured logging with correlation IDs

#### Testing & Quality
- ✅ Comprehensive unit test suite (205 tests)
- ✅ >80% code coverage
- ✅ AutoFixture for test data
- ✅ Moq for mocking
- ✅ FluentAssertions for readability

#### Configuration & Build
- ✅ Centralised versioning (Directory.Build.props)
- ✅ EditorConfig for code consistency
- ✅ Nullable reference types enabled
- ✅ Code analysis with .NET analysers

## Troubleshooting

### Common Issues

#### Port Already in Use
**Problem**: `Address already in use` error when starting the application.

**Solution**:
```bash
# Option 1: Use a different port
dotnet run --project MyWebApp --urls "https://localhost:7001;http://localhost:5001"

# Option 2: Find and kill the process using the port (Windows)
netstat -ano | findstr :7xxx
taskkill /PID <PID> /F

# Option 2: Find and kill the process using the port (Linux/Mac)
lsof -ti:7xxx | xargs kill
```

#### Test Failures
**Problem**: Tests fail after pulling latest changes.

**Solution**:
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build

# Run tests with detailed output
dotnet test --verbosity normal
```

#### Build Errors
**Problem**: Build fails with namespace or reference errors.

**Solution**:
```bash
# Verify all projects restore correctly
dotnet restore --force

# Check for package conflicts
dotnet list package --include-transitive

# Rebuild from scratch
dotnet clean
dotnet build --no-incremental
```

#### Swagger Not Loading
**Problem**: Swagger UI shows 404 or doesn't load.

**Solution**:
- Verify you're running in Development mode
- Check `launchSettings.json` for correct environment
- Ensure `app.UseSwagger()` is called before `app.UseSwaggerUI()`

## Versioning

The solution uses **Semantic Versioning (SemVer)** via `Directory.Build.props`:

- **Current Version**: `1.2.3`
- **Format**: `MAJOR.MINOR.PATCH`
- **Assembly Version**: `1.0.0.0` (changes only with breaking changes)
- **File Version**: `1.2.3.0` (includes build metadata)

### Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.2.3 | Nov 2025 | Phase 3 complete - 205 tests passing |
| 1.2.0 | Nov 2025 | Phase 2 complete - Business logic and validation |
| 1.0.0 | Nov 2025 | Phase 1 - Initial clean architecture foundation |

## Contributing

This is a personal learning and demonstration project. Whilst it's not actively seeking contributions, you're welcome to:

- Fork the project for your own learning
- Use it as a template for your projects
- Provide feedback or suggestions via issues

### Development Guidelines

If forking or extending this project:

1. Follow the existing code style and conventions
2. Maintain >80% test coverage for new features
3. Update documentation for significant changes
4. Use semantic versioning for releases
5. Keep the clean architecture boundaries clear

## Licence

This project is for learning and demonstration purposes. See the project root for licence information.

## Acknowledgements

- **Microsoft**: .NET platform and comprehensive documentation
- **Community**: Open-source libraries (xUnit, Moq, FluentValidation, AutoFixture)
- **W3C**: Trace Context specification

## Project Information

- **Project Name**: MyWebApp (Azure-Project-001)
- **Purpose**: Learning modern .NET development and Azure deployment
- **Target Platform**: Azure App Service
- **Framework**: .NET 8.0 (LTS)
- **Status**: Production-ready core features, Phase 3 complete

---

**Last Updated**: November 2025  
**Version**: 1.2.3  
**Test Suite**: 205 tests passing ✅  
**Coverage**: >80% across all layers
