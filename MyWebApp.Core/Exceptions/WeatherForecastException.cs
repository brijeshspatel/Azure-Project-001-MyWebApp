using System.Runtime.Serialization;

namespace MyWebApp.Core.Exceptions;

/// <summary>
/// Exception thrown when weather forecast operations encounter domain-specific errors.
/// </summary>
[Serializable]
public class WeatherForecastException : DomainException
{
    /// <summary>
    /// Gets the number of days that caused the exception (if applicable).
    /// </summary>
    public int? RequestedDays { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class.
    /// </summary>
    public WeatherForecastException()
        : base("An error occurred whilst processing the weather forecast request.", "WF000")
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public WeatherForecastException(string message)
        : base(message, "WF000")
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class with a specified error message and error code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    public WeatherForecastException(string message, string errorCode)
        : base(message, errorCode)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class with requested days information.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    /// <param name="requestedDays">The number of days that caused the exception.</param>
    public WeatherForecastException(string message, string errorCode, int requestedDays)
        : base(message, errorCode)
    {
        RequestedDays = requestedDays;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public WeatherForecastException(string message, Exception innerException)
        : base(message, "WF000", innerException)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class with a specified error message, error code, and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public WeatherForecastException(string message, string errorCode, Exception innerException)
        : base(message, errorCode, innerException)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="WeatherForecastException"/> class with serialised data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialised object data.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information.</param>
    [Obsolete("This constructor is obsolete due to BinaryFormatter deprecation.")]
    protected WeatherForecastException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        RequestedDays = (int?)info.GetValue(nameof(RequestedDays), typeof(int?));
    }

    /// <summary>
    /// Sets the <see cref="SerializationInfo"/> with information about the exception.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialised object data.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information.</param>
    [Obsolete("This method is obsolete due to BinaryFormatter deprecation.")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(RequestedDays), RequestedDays);
    }

    /// <summary>
    /// Creates an exception for invalid day range requests.
    /// </summary>
    /// <param name="days">The invalid number of days requested.</param>
    /// <param name="minDays">The minimum allowed number of days.</param>
    /// <param name="maxDays">The maximum allowed number of days.</param>
    /// <returns>A new instance of <see cref="WeatherForecastException"/>.</returns>
    public static WeatherForecastException InvalidDayRange(int days, int minDays, int maxDays)
    {
        return new WeatherForecastException(
            $"The requested number of days ({days}) is outside the valid range of {minDays} to {maxDays}.",
            "WF001",
            days);
    }

    /// <summary>
    /// Creates an exception for when forecast data is unavailable.
    /// </summary>
    /// <param name="reason">The reason the forecast is unavailable.</param>
    /// <returns>A new instance of <see cref="WeatherForecastException"/>.</returns>
    public static WeatherForecastException ForecastUnavailable(string reason)
    {
        return new WeatherForecastException(
            $"Weather forecast is currently unavailable: {reason}",
            "WF002");
    }
}
