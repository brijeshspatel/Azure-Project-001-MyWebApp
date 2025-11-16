using Microsoft.AspNetCore.Mvc;
using MyWebApp.Core.Interfaces;

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
    /// Gets weather forecasts for the specified number of days.
    /// </summary>
    /// <param name="days">The number of days to forecast (default is 5, max is 30).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of weather forecasts.</returns>
    /// <response code="200">Returns the weather forecasts.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] int days = 5, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received request for weather forecasts for {Days} days", days);

        var forecasts = await _weatherForecastService.GetForecastsAsync(days, cancellationToken);

        return Ok(forecasts);
    }
}
