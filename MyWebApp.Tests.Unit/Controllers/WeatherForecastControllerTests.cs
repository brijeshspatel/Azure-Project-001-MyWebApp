using Azure_Project_001_MyWebApp.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyWebApp.Core.Interfaces;
using MyWebApp.Core.Models;
using MyWebApp.Core.Models.Requests;
using Xunit;

namespace MyWebApp.Tests.Unit.Controllers;

/// <summary>
/// Tests for <see cref="WeatherForecastController"/>.
/// </summary>
public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _mockService;
    private readonly Mock<ILogger<WeatherForecastController>> _mockLogger;
    private readonly WeatherForecastController _controller;

    public WeatherForecastControllerTests()
    {
        _mockService = new Mock<IWeatherForecastService>();
        _mockLogger = new Mock<ILogger<WeatherForecastController>>();
        _controller = new WeatherForecastController(_mockService.Object, _mockLogger.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullService_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new WeatherForecastController(null!, _mockLogger.Object);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("weatherForecastService");
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new WeatherForecastController(_mockService.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var controller = new WeatherForecastController(_mockService.Object, _mockLogger.Object);

        // Assert
        controller.Should().NotBeNull();
        controller.Should().BeAssignableTo<ControllerBase>();
    }

    #endregion

    #region Get Method - Success Tests

    [Fact]
    public async Task Get_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Get_WithValidRequest_ReturnsForecasts()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedForecasts = okResult.Value.Should().BeAssignableTo<IEnumerable<WeatherForecastResponse>>().Subject;
        returnedForecasts.Should().HaveCount(5);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(30)]
    public async Task Get_WithDifferentDays_ReturnsCorrectCount(int days)
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = days };
        var forecasts = CreateSampleForecasts(days);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedForecasts = okResult.Value.Should().BeAssignableTo<IEnumerable<WeatherForecastResponse>>().Subject;
        returnedForecasts.Should().HaveCount(days);
    }

    [Fact]
    public async Task Get_WithLocation_ReturnsForecasts()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5, Location = "London" };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Get_WithIncludeDetails_ReturnsForecasts()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5, IncludeDetails = true };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Get_WithAllParameters_ReturnsForecasts()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 10, 
            Location = "Manchester", 
            IncludeDetails = true 
        };
        var forecasts = CreateSampleForecasts(10);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedForecasts = okResult.Value.Should().BeAssignableTo<IEnumerable<WeatherForecastResponse>>().Subject;
        returnedForecasts.Should().HaveCount(10);
    }

    #endregion

    #region Get Method - Logging Tests

    [Fact]
    public async Task Get_WithValidRequest_LogsInformation()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        await _controller.Get(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received request for weather forecasts")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithLocation_LogsLocation()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5, Location = "Birmingham" };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        await _controller.Get(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Birmingham")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithoutLocation_DoesNotLogLocation()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5, Location = null };
        var forecasts = CreateSampleForecasts(5);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        await _controller.Get(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("5 days")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Get Method - Service Integration Tests

    [Fact]
    public async Task Get_CallsServiceWithRequest()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 7 };
        var forecasts = CreateSampleForecasts(7);
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecasts);

        // Act
        await _controller.Get(request);

        // Assert
        _mockService.Verify(
            x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Get_WithCancellationToken_PassesToService()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var forecasts = CreateSampleForecasts(5);
        var cancellationToken = new CancellationToken();
        _mockService.Setup(x => x.GetForecastsAsync(request, cancellationToken))
            .ReturnsAsync(forecasts);

        // Act
        await _controller.Get(request, cancellationToken);

        // Assert
        _mockService.Verify(
            x => x.GetForecastsAsync(request, cancellationToken),
            Times.Once);
    }

    #endregion

    #region Get Method - Return Value Tests

    [Fact]
    public async Task Get_ReturnsCorrectForecastData()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 3 };
        var expectedForecasts = new List<WeatherForecastResponse>
        {
            new() { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), TemperatureC = 15, TemperatureF = 59, Summary = "Mild" },
            new() { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), TemperatureC = 20, TemperatureF = 68, Summary = "Warm" },
            new() { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), TemperatureC = 10, TemperatureF = 50, Summary = "Cool" }
        };
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedForecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedForecasts = okResult.Value.Should().BeAssignableTo<IEnumerable<WeatherForecastResponse>>().Subject.ToList();
        
        returnedForecasts.Should().HaveCount(3);
        returnedForecasts[0].TemperatureC.Should().Be(15);
        returnedForecasts[0].Summary.Should().Be("Mild");
        returnedForecasts[1].TemperatureC.Should().Be(20);
        returnedForecasts[1].Summary.Should().Be("Warm");
        returnedForecasts[2].TemperatureC.Should().Be(10);
        returnedForecasts[2].Summary.Should().Be("Cool");
    }

    [Fact]
    public async Task Get_WithEmptyForecasts_ReturnsEmptyCollection()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };
        var emptyForecasts = new List<WeatherForecastResponse>();
        _mockService.Setup(x => x.GetForecastsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyForecasts);

        // Act
        var result = await _controller.Get(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedForecasts = okResult.Value.Should().BeAssignableTo<IEnumerable<WeatherForecastResponse>>().Subject;
        returnedForecasts.Should().BeEmpty();
    }

    #endregion

    #region HTTP Attribute Tests

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {
        // Assert
        var controllerType = typeof(WeatherForecastController);
        controllerType.Should().BeDecoratedWith<ApiControllerAttribute>();
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {
        // Assert
        var controllerType = typeof(WeatherForecastController);
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false)
            .Cast<RouteAttribute>()
            .FirstOrDefault();
        
        routeAttribute.Should().NotBeNull();
        routeAttribute!.Template.Should().Be("api/[controller]");
    }

    [Fact]
    public void Get_HasHttpGetAttribute()
    {
        // Arrange
        var methodInfo = typeof(WeatherForecastController)
            .GetMethod(nameof(WeatherForecastController.Get));

        // Assert
        methodInfo.Should().NotBeNull();
        methodInfo!.Should().BeDecoratedWith<HttpGetAttribute>();
    }

    [Fact]
    public void Get_HttpGetAttribute_HasName()
    {
        // Arrange
        var methodInfo = typeof(WeatherForecastController)
            .GetMethod(nameof(WeatherForecastController.Get));

        var httpGetAttribute = methodInfo!.GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .FirstOrDefault();

        // Assert
        httpGetAttribute.Should().NotBeNull();
        httpGetAttribute!.Name.Should().Be("GetWeatherForecast");
    }

    [Fact]
    public void Get_HasProducesResponseTypeAttributes()
    {
        // Arrange
        var methodInfo = typeof(WeatherForecastController)
            .GetMethod(nameof(WeatherForecastController.Get));

        var producesAttributes = methodInfo!.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false)
            .Cast<ProducesResponseTypeAttribute>()
            .ToList();

        // Assert
        producesAttributes.Should().HaveCount(3);
        producesAttributes.Should().Contain(x => x.StatusCode == 200);
        producesAttributes.Should().Contain(x => x.StatusCode == 400);
        producesAttributes.Should().Contain(x => x.StatusCode == 500);
    }

    #endregion

    #region Helper Methods

    private static List<WeatherForecastResponse> CreateSampleForecasts(int count)
    {
        var forecasts = new List<WeatherForecastResponse>();
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        for (int i = 1; i <= count; i++)
        {
            forecasts.Add(new WeatherForecastResponse
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                TemperatureC = Random.Shared.Next(-20, 55),
                TemperatureF = 32 + Random.Shared.Next(-4, 99),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            });
        }

        return forecasts;
    }

    #endregion
}
