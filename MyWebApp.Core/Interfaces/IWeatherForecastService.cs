using MyWebApp.Core.Models;
using MyWebApp.Core.Models.Requests;

namespace MyWebApp.Core.Interfaces;

/// <summary>
/// Interface for weather forecast service operations.
/// </summary>
public interface IWeatherForecastService
{
    /// <summary>
    /// Retrieves a collection of weather forecasts based on the request parameters.
    /// </summary>
    /// <param name="request">The request containing forecast parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>A collection of weather forecast responses.</returns>
    /// <exception cref="Exceptions.ValidationException">Thrown when the request fails validation.</exception>
    /// <exception cref="Exceptions.WeatherForecastException">Thrown when forecast generation fails.</exception>
    Task<IEnumerable<WeatherForecastResponse>> GetForecastsAsync(
        GetWeatherForecastRequest request, 
        CancellationToken cancellationToken = default);
}
