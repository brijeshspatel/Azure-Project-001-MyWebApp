using MyWebApp.Core.Models;

namespace MyWebApp.Core.Interfaces;

/// <summary>
/// Interface for weather forecast service operations.
/// </summary>
public interface IWeatherForecastService
{
    /// <summary>
    /// Retrieves a collection of weather forecasts.
    /// </summary>
    /// <param name="days">The number of days to forecast (default is 5).</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>A collection of weather forecast responses.</returns>
    Task<IEnumerable<WeatherForecastResponse>> GetForecastsAsync(int days = 5, CancellationToken cancellationToken = default);
}
