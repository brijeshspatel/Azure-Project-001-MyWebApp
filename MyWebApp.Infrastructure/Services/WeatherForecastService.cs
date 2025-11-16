using Microsoft.Extensions.Logging;
using MyWebApp.Core.Interfaces;
using MyWebApp.Core.Models;

namespace MyWebApp.Infrastructure.Services;

/// <summary>
/// Implementation of weather forecast service.
/// </summary>
public class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(ILogger<WeatherForecastService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task<IEnumerable<WeatherForecastResponse>> GetForecastsAsync(int days = 5, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating weather forecasts for {Days} days", days);

        if (days <= 0)
        {
            _logger.LogWarning("Invalid number of days requested: {Days}. Defaulting to 5", days);
            days = 5;
        }

        if (days > 30)
        {
            _logger.LogWarning("Too many days requested: {Days}. Limiting to 30", days);
            days = 30;
        }

        var forecasts = Enumerable.Range(1, days).Select(index =>
        {
            var forecast = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            return new WeatherForecastResponse
            {
                Date = forecast.Date,
                TemperatureC = forecast.TemperatureC,
                TemperatureF = forecast.TemperatureF,
                Summary = forecast.Summary
            };
        }).ToList();

        _logger.LogInformation("Successfully generated {Count} weather forecasts", forecasts.Count);

        return Task.FromResult<IEnumerable<WeatherForecastResponse>>(forecasts);
    }
}
