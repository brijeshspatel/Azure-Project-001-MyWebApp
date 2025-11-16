namespace MyWebApp.Core.Models.Requests;

/// <summary>
/// Request model for retrieving weather forecasts.
/// </summary>
public class GetWeatherForecastRequest
{
    /// <summary>
    /// Gets or sets the number of days to forecast.
    /// </summary>
    /// <value>
    /// The number of days for the forecast. Must be between 1 and 30. Default is 5.
    /// </value>
    public int Days { get; set; } = 5;

    /// <summary>
    /// Gets or sets the optional location for the weather forecast.
    /// </summary>
    /// <value>
    /// The location name. If not specified, a default location is used.
    /// </value>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets whether to include detailed information in the response.
    /// </summary>
    /// <value>
    /// True to include extended details; otherwise, false. Default is false.
    /// </value>
    public bool IncludeDetails { get; set; } = false;
}
