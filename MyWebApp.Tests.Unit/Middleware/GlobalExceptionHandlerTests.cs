using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyWebApp.Core.Exceptions;
using Xunit;

namespace MyWebApp.Tests.Unit.Middleware;

/// <summary>
/// Tests for <see cref="Azure_Project_001_MyWebApp.Middleware.GlobalExceptionHandler"/>.
/// </summary>
[SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "Test code needs to test handling of generic exceptions")]
public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<Azure_Project_001_MyWebApp.Middleware.GlobalExceptionHandler>> _mockLogger;
    private readonly Azure_Project_001_MyWebApp.Middleware.GlobalExceptionHandler _sut;
    private readonly DefaultHttpContext _httpContext;

    public GlobalExceptionHandlerTests()
    {
        _mockLogger = new Mock<ILogger<Azure_Project_001_MyWebApp.Middleware.GlobalExceptionHandler>>();
        _sut = new Azure_Project_001_MyWebApp.Middleware.GlobalExceptionHandler(_mockLogger.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new Azure_Project_001_MyWebApp.Middleware.GlobalExceptionHandler(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public async Task TryHandleAsync_WithNullHttpContext_ThrowsArgumentNullException()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var act = async () => await _sut.TryHandleAsync(null!, exception, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("httpContext");
    }

    [Fact]
    public async Task TryHandleAsync_WithNullException_ThrowsArgumentNullException()
    {
        // Act
        var act = async () => await _sut.TryHandleAsync(_httpContext, null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("exception");
    }

    [Fact]
    public async Task TryHandleAsync_WithValidationException_Returns400BadRequest()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Days", new[] { "Days must be between 1 and 30" } }
        };
        var exception = new ValidationException(errors);

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_WithValidationException_IncludesValidationErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Days", new[] { "Days must be between 1 and 30" } },
            { "Location", new[] { "Location is too long" } }
        };
        var exception = new ValidationException(errors);

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(400);
        problemDetails.Title.Should().Be("One or more validation errors occurred.");
        problemDetails.Extensions.Should().ContainKey("errors");
    }

    [Fact]
    public async Task TryHandleAsync_WithNotFoundException_Returns404NotFound()
    {
        // Arrange
        var exception = new NotFoundException("WeatherForecast", "123");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task TryHandleAsync_WithNotFoundException_IncludesEntityDetails()
    {
        // Arrange
        var exception = new NotFoundException("WeatherForecast", "ABC123");

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(404);
        problemDetails.Title.Should().Be("Resource not found.");
        problemDetails.Extensions.Should().ContainKey("entityType");
        problemDetails.Extensions.Should().ContainKey("entityId");
    }

    [Fact]
    public async Task TryHandleAsync_WithWeatherForecastException_Returns400BadRequest()
    {
        // Arrange
        var exception = new WeatherForecastException("Forecast generation failed", "WF001");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_WithWeatherForecastException_IncludesRequestedDays()
    {
        // Arrange
        var exception = new WeatherForecastException("Too many days requested", "WF002", 50);

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Extensions.Should().ContainKey("requestedDays");
        problemDetails.Extensions.Should().ContainKey("errorCode");
    }

    [Fact]
    public async Task TryHandleAsync_WithDomainException_Returns400BadRequest()
    {
        // Arrange
        var exception = new DomainException("Domain error occurred", "DOM001");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_WithGenericException_Returns500InternalServerError()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected error");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task TryHandleAsync_WithGenericException_HidesInternalDetails()
    {
        // Arrange
        var exception = new InvalidOperationException("Sensitive internal error details");

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(500);
        problemDetails.Title.Should().Be("An error occurred while processing your request.");
        problemDetails.Detail.Should().Be("An unexpected error occurred. Please try again later.");
        problemDetails.Detail.Should().NotContain("Sensitive");
    }

    [Fact]
    public async Task TryHandleAsync_IncludesTraceIdInResponse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _httpContext.TraceIdentifier = "test-trace-id-12345";

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Extensions.Should().ContainKey("traceId");
    }

    [Fact]
    public async Task TryHandleAsync_WithActivity_IncludesW3CTraceContext()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var activity = new Activity("test-operation");
        activity.Start();
        Activity.Current = activity;

        try
        {
            // Act
            await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            _httpContext.Response.Body.Position = 0;
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            problemDetails.Should().NotBeNull();
            problemDetails!.Extensions.Should().ContainKey("traceId");
            problemDetails.Extensions.Should().ContainKey("spanId");
        }
        finally
        {
            activity.Stop();
            Activity.Current = null;
        }
    }

    [Fact]
    public async Task TryHandleAsync_WithParentActivity_IncludesParentSpanId()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var parentActivity = new Activity("parent-operation");
        parentActivity.Start();
        
        var childActivity = new Activity("child-operation");
        if (parentActivity.Id != null)
        {
            childActivity.SetParentId(parentActivity.Id);
        }
        childActivity.Start();
        Activity.Current = childActivity;

        try
        {
            // Act
            await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            // Assert
            _httpContext.Response.Body.Position = 0;
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            problemDetails.Should().NotBeNull();
            problemDetails!.Extensions.Should().ContainKey("traceId");
            problemDetails.Extensions.Should().ContainKey("spanId");
            problemDetails.Extensions.Should().ContainKey("parentSpanId");
        }
        finally
        {
            childActivity.Stop();
            parentActivity.Stop();
            Activity.Current = null;
        }
    }

    [Fact]
    public async Task TryHandleAsync_LogsException()
    {
        // Arrange
        var exception = new Exception("Test logging");

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An exception occurred")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_SetsCorrectRequestInstance()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _httpContext.Request.Path = "/api/weatherforecast";

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Instance.Should().Be("/api/weatherforecast");
    }

    [Fact]
    public async Task TryHandleAsync_AlwaysReturnsTrue()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue("exception handler should always handle exceptions");
    }

    [Fact]
    public async Task TryHandleAsync_WithCancellationToken_HandlesRequest()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var cts = new CancellationTokenSource();

        // Act
        var result = await _sut.TryHandleAsync(_httpContext, exception, cts.Token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task TryHandleAsync_WithValidationExceptionWithErrorCode_IncludesErrorCode()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Days", new[] { "Invalid days" } }
        };
        var exception = new ValidationException(errors);

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.Body.Position = 0;
        var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        problemDetails.Should().NotBeNull();
        problemDetails!.Extensions.Should().ContainKey("errorCode");
    }

    [Fact]
    public async Task TryHandleAsync_ResponseIncludesJsonContent()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        await _sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        // Assert
        _httpContext.Response.ContentType.Should().Contain("json");
    }
}
