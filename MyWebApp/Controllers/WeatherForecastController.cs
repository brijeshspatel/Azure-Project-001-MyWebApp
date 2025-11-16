using Microsoft.AspNetCore.Mvc;
using MyWebApp.Core.Interfaces;
using MyWebApp.Core.Models.Requests;

namespace Azure_Project_001_MyWebApp.Controllers;

/// <summary>
/// Controller for weather forecast operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="weatherForecastService">The weather forecast service.</param>
    /// <param name="logger">The logger instance.</param>
    public WeatherForecastController(
        IWeatherForecastService weatherForecastService,
        ILogger<WeatherForecastController> logger)
    {
        _weatherForecastService = weatherForecastService ?? throw new ArgumentNullException(nameof(weatherForecastService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets weather forecasts for the specified parameters.
    /// </summary>
    /// <param name="request">The forecast request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of weather forecasts.</returns>
    /// <response code="200">Returns the weather forecasts.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an error occurs processing the request.</response>
    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(
        [FromQuery] GetWeatherForecastRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Received request for weather forecasts: {Days} days{Location}",
            request.Days,
            string.IsNullOrWhiteSpace(request.Location) ? string.Empty : $", Location: {request.Location}");

        var forecasts = await _weatherForecastService.GetForecastsAsync(request, cancellationToken);

        return Ok(forecasts);
    }
}
