using FluentValidation;
using Microsoft.Extensions.Logging;
using MyWebApp.Core.Exceptions;
using MyWebApp.Core.Interfaces;
using MyWebApp.Core.Models;
using MyWebApp.Core.Models.Requests;

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
    private readonly IValidator<GetWeatherForecastRequest> _validator;

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="validator">The validator for weather forecast requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger or validator is null.</exception>
    public WeatherForecastService(
        ILogger<WeatherForecastService> logger,
        IValidator<GetWeatherForecastRequest> validator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WeatherForecastResponse>> GetForecastsAsync(
        GetWeatherForecastRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation(
            "Generating weather forecasts for {Days} days{Location}",
            request.Days,
            string.IsNullOrWhiteSpace(request.Location) ? string.Empty : $" for {request.Location}");

        // Validate the request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            _logger.LogWarning("Validation failed for weather forecast request: {Errors}", errors);
            throw new MyWebApp.Core.Exceptions.ValidationException(errors);
        }

        try
        {
            var forecasts = Enumerable.Range(1, request.Days).Select(index =>
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

            return forecasts;
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            _logger.LogError(ex, "Unexpected error generating weather forecasts");
            throw new WeatherForecastException(
                "An unexpected error occurred whilst generating the weather forecast.",
                "WF999",
                ex);
        }
    }
}
