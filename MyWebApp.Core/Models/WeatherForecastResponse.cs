namespace MyWebApp.Core.Models;

/// <summary>
/// Response model for weather forecast data transfer.
/// </summary>
public class WeatherForecastResponse
{
    /// <summary>
    /// Gets or sets the date of the forecast.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the temperature in Celsius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Gets or sets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF { get; set; }

    /// <summary>
    /// Gets or sets a textual summary of the weather conditions.
    /// </summary>
    public string? Summary { get; set; }
}
