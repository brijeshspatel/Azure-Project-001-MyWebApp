using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using MyWebApp.Core.Exceptions;
using MyWebApp.Core.Interfaces;
using MyWebApp.Core.Models.Requests;
using MyWebApp.Infrastructure.Services;
using Xunit;

namespace MyWebApp.Tests.Unit.Infrastructure.Services;

/// <summary>
/// Tests for <see cref="WeatherForecastService"/>.
/// </summary>
public class WeatherForecastServiceTests
{
    private readonly Mock<ILogger<WeatherForecastService>> _mockLogger;
    private readonly Mock<IValidator<GetWeatherForecastRequest>> _mockValidator;
    private readonly WeatherForecastService _service;

    public WeatherForecastServiceTests()
    {
        _mockLogger = new Mock<ILogger<WeatherForecastService>>();
        _mockValidator = new Mock<IValidator<GetWeatherForecastRequest>>();
        _service = new WeatherForecastService(_mockLogger.Object, _mockValidator.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new WeatherForecastService(null!, _mockValidator.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public void Constructor_WithNullValidator_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new WeatherForecastService(_mockLogger.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("validator");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var service = new WeatherForecastService(_mockLogger.Object, _mockValidator.Object);

        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IWeatherForecastService>();
    }

    #endregion

    #region GetForecastsAsync - Valid Request Tests

    [Fact]
    public async Task GetForecastsAsync_WithValidRequest_ReturnsForecasts()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    public async Task GetForecastsAsync_WithDifferentDays_ReturnsCorrectCount(int days)
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = days };
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        result.Should().HaveCount(days);
    }

    [Fact]
    public async Task GetForecastsAsync_WithValidRequest_ReturnsValidForecastData()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 3 };
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        var forecasts = result.ToList();
        forecasts.Should().AllSatisfy(forecast =>
        {
            forecast.Date.Should().BeAfter(DateOnly.FromDateTime(DateTime.UtcNow));
            forecast.TemperatureC.Should().BeInRange(-20, 54);
            forecast.TemperatureF.Should().NotBe(0);
            forecast.Summary.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetForecastsAsync_WithValidRequest_ReturnsFutureDates()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        result.Should().AllSatisfy(forecast =>
        {
            forecast.Date.Should().BeAfter(today);
        });
    }

    [Fact]
    public async Task GetForecastsAsync_WithValidRequest_ReturnsSequentialDates()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        var forecasts = result.OrderBy(f => f.Date).ToList();
        for (int i = 1; i < forecasts.Count; i++)
        {
            var expectedDate = forecasts[i - 1].Date.AddDays(1);
            forecasts[i].Date.Should().Be(expectedDate);
        }
    }

    [Fact]
    public async Task GetForecastsAsync_WithLocation_LogsLocation()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5, Location = "London" };
        SetupValidValidation();

        // Act
        await _service.GetForecastsAsync(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("London")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetForecastsAsync_WithoutLocation_DoesNotLogLocation()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5, Location = null };
        SetupValidValidation();

        // Act
        await _service.GetForecastsAsync(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("for 5 days")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetForecastsAsync_WithValidRequest_LogsSuccessMessage()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 3 };
        SetupValidValidation();

        // Act
        await _service.GetForecastsAsync(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully generated 3 weather forecasts")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region GetForecastsAsync - Validation Tests

    [Fact]
    public async Task GetForecastsAsync_WithInvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 0 };
        SetupInvalidValidation("Days", "Days must be at least 1");

        // Act
        var act = async () => await _service.GetForecastsAsync(request);

        // Assert
        await act.Should().ThrowAsync<MyWebApp.Core.Exceptions.ValidationException>()
            .WithMessage("*validation*");
    }

    [Fact]
    public async Task GetForecastsAsync_WithInvalidRequest_ValidationExceptionContainsErrors()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 0 };
        SetupInvalidValidation("Days", "Days must be at least 1");

        // Act
        var act = async () => await _service.GetForecastsAsync(request);

        // Assert
        var exception = await act.Should().ThrowAsync<MyWebApp.Core.Exceptions.ValidationException>();
        exception.Which.Errors.Should().ContainKey("Days");
        exception.Which.Errors["Days"].Should().Contain("Days must be at least 1");
    }

    [Fact]
    public async Task GetForecastsAsync_WithMultipleValidationErrors_ThrowsValidationExceptionWithAllErrors()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 0, Location = "Invalid@Location" };
        SetupMultipleValidationErrors();

        // Act
        var act = async () => await _service.GetForecastsAsync(request);

        // Assert
        var exception = await act.Should().ThrowAsync<MyWebApp.Core.Exceptions.ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
        exception.Which.Errors.Should().ContainKey("Days");
        exception.Which.Errors.Should().ContainKey("Location");
    }

    [Fact]
    public async Task GetForecastsAsync_WithValidationFailure_LogsWarning()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 0 };
        SetupInvalidValidation("Days", "Days must be at least 1");

        // Act
        try
        {
            await _service.GetForecastsAsync(request);
        }
        catch (MyWebApp.Core.Exceptions.ValidationException)
        {
            // Expected
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetForecastsAsync_CallsValidator()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        SetupValidValidation();

        // Act
        await _service.GetForecastsAsync(request);

        // Assert
        _mockValidator.Verify(
            x => x.ValidateAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetForecastsAsync - Exception Handling Tests

    [Fact]
    public async Task GetForecastsAsync_WhenDomainExceptionOccurs_DoesNotWrapException()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var domainException = new WeatherForecastException("Domain error", "WF001");
        _mockValidator.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(domainException);

        // Act
        var act = async () => await _service.GetForecastsAsync(request);

        // Assert
        var exception = await act.Should().ThrowAsync<WeatherForecastException>();
        exception.Which.Should().Be(domainException);
        exception.Which.ErrorCode.Should().Be("WF001");
    }

    #endregion

    #region Temperature Conversion Tests

    [Fact]
    public async Task GetForecastsAsync_ReturnsCorrectTemperatureConversion()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 10 };
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        result.Should().AllSatisfy(forecast =>
        {
            // Verify Fahrenheit conversion is consistent
            var expectedF = 32 + (int)(forecast.TemperatureC / 0.5556);
            forecast.TemperatureF.Should().Be(expectedF);
        });
    }

    #endregion

    #region Summary Tests

    [Fact]
    public async Task GetForecastsAsync_ReturnsSummaryFromPredefinedList()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 20 };
        var validSummaries = new[] 
        { 
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", 
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching" 
        };
        SetupValidValidation();

        // Act
        var result = await _service.GetForecastsAsync(request);

        // Assert
        result.Should().AllSatisfy(forecast =>
        {
            forecast.Summary.Should().BeOneOf(validSummaries);
        });
    }

    #endregion

    #region Helper Methods

    private void SetupValidValidation()
    {
        _mockValidator.Setup(x => x.ValidateAsync(
                It.IsAny<GetWeatherForecastRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupInvalidValidation(string propertyName, string errorMessage)
    {
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure(propertyName, errorMessage)
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockValidator.Setup(x => x.ValidateAsync(
                It.IsAny<GetWeatherForecastRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
    }

    private void SetupMultipleValidationErrors()
    {
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Days", "Days must be at least 1"),
            new ValidationFailure("Location", "Location contains invalid characters")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockValidator.Setup(x => x.ValidateAsync(
                It.IsAny<GetWeatherForecastRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
    }

    #endregion
}
