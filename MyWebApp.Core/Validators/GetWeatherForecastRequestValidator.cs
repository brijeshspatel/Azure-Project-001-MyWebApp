using FluentValidation;
using MyWebApp.Core.Models.Requests;

namespace MyWebApp.Core.Validators;

/// <summary>
/// Validator for <see cref="GetWeatherForecastRequest"/>.
/// </summary>
public class GetWeatherForecastRequestValidator : AbstractValidator<GetWeatherForecastRequest>
{
    /// <summary>
    /// Minimum number of days allowed for a forecast.
    /// </summary>
    public const int MinDays = 1;

    /// <summary>
    /// Maximum number of days allowed for a forecast.
    /// </summary>
    public const int MaxDays = 30;

    /// <summary>
    /// Maximum length for the location name.
    /// </summary>
    public const int MaxLocationLength = 100;

    /// <summary>
    /// Initialises a new instance of the <see cref="GetWeatherForecastRequestValidator"/> class.
    /// </summary>
    public GetWeatherForecastRequestValidator()
    {
        RuleFor(x => x.Days)
            .GreaterThanOrEqualTo(MinDays)
            .WithMessage($"The number of days must be at least {MinDays}.")
            .WithErrorCode("WF_DAYS_TOO_LOW")
            .LessThanOrEqualTo(MaxDays)
            .WithMessage($"The number of days cannot exceed {MaxDays}.")
            .WithErrorCode("WF_DAYS_TOO_HIGH");

        RuleFor(x => x.Location)
            .MaximumLength(MaxLocationLength)
            .WithMessage($"The location name cannot exceed {MaxLocationLength} characters.")
            .WithErrorCode("WF_LOCATION_TOO_LONG")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));

        RuleFor(x => x.Location)
            .Must(BeValidLocationFormat)
            .WithMessage("The location name contains invalid characters. Only letters, numbers, spaces, and basic punctuation are allowed.")
            .WithErrorCode("WF_LOCATION_INVALID")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));
    }

    /// <summary>
    /// Validates that a location name has a valid format.
    /// </summary>
    /// <param name="location">The location name to validate.</param>
    /// <returns>True if the location is valid; otherwise, false.</returns>
    private static bool BeValidLocationFormat(string? location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return true;
        }

        // Allow letters, numbers, spaces, hyphens, apostrophes, and commas
        return location.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '\'' || c == ',');
    }
}
