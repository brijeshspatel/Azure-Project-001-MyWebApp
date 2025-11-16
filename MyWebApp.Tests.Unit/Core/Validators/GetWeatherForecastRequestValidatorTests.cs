using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using MyWebApp.Core.Models.Requests;
using MyWebApp.Core.Validators;
using Xunit;

namespace MyWebApp.Tests.Unit.Core.Validators;

/// <summary>
/// Tests for <see cref="GetWeatherForecastRequestValidator"/>.
/// </summary>
public class GetWeatherForecastRequestValidatorTests
{
    private readonly GetWeatherForecastRequestValidator _validator;

    public GetWeatherForecastRequestValidatorTests()
    {
        _validator = new GetWeatherForecastRequestValidator();
    }

    #region Days Validation Tests

    [Fact]
    public async Task Validate_WithDefaultDays_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest();

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(30)]
    public async Task Validate_WithValidDays_IsValid(int days)
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = days };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithMinimumDays_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = GetWeatherForecastRequestValidator.MinDays 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithMaximumDays_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = GetWeatherForecastRequestValidator.MaxDays 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public async Task Validate_WithDaysBelowMinimum_HasValidationError(int days)
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = days };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Days)
            .WithErrorCode("WF_DAYS_TOO_LOW")
            .WithErrorMessage($"The number of days must be at least {GetWeatherForecastRequestValidator.MinDays}.");
    }

    [Theory]
    [InlineData(31)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(365)]
    public async Task Validate_WithDaysAboveMaximum_HasValidationError(int days)
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = days };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Days)
            .WithErrorCode("WF_DAYS_TOO_HIGH")
            .WithErrorMessage($"The number of days cannot exceed {GetWeatherForecastRequestValidator.MaxDays}.");
    }

    #endregion

    #region Location Validation Tests

    [Fact]
    public async Task Validate_WithNullLocation_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = null 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithEmptyLocation_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = string.Empty 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithWhitespaceLocation_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = "   " 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("London")]
    [InlineData("New York")]
    [InlineData("San Francisco")]
    [InlineData("Manchester")]
    [InlineData("Birmingham")]
    public async Task Validate_WithValidLocation_IsValid(string location)
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = location 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Saint Marys")]
    [InlineData("O'Brien")]
    [InlineData("London, UK")]
    [InlineData("New York, NY")]
    [InlineData("Saint-Tropez")]
    public async Task Validate_WithValidLocationWithPunctuation_IsValid(string location)
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = location 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithLocationAtMaxLength_IsValid()
    {
        // Arrange
        var location = new string('A', GetWeatherForecastRequestValidator.MaxLocationLength);
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = location 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithLocationExceedingMaxLength_HasValidationError()
    {
        // Arrange
        var location = new string('A', GetWeatherForecastRequestValidator.MaxLocationLength + 1);
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = location 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorCode("WF_LOCATION_TOO_LONG")
            .WithErrorMessage($"The location name cannot exceed {GetWeatherForecastRequestValidator.MaxLocationLength} characters.");
    }

    [Theory]
    [InlineData("London@")]
    [InlineData("New#York")]
    [InlineData("Paris!")]
    [InlineData("Berlin$")]
    [InlineData("Rome%")]
    [InlineData("Madrid&")]
    [InlineData("Barcelona*")]
    [InlineData("Munich(")]
    [InlineData("Vienna)")]
    [InlineData("Prague+")]
    [InlineData("Budapest=")]
    [InlineData("Warsaw[")]
    [InlineData("Lisbon]")]
    [InlineData("Athens{")]
    [InlineData("Dublin}")]
    public async Task Validate_WithInvalidLocationCharacters_HasValidationError(string location)
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = location 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorCode("WF_LOCATION_INVALID")
            .WithErrorMessage("The location name contains invalid characters. Only letters, numbers, spaces, and basic punctuation are allowed.");
    }

    [Theory]
    [InlineData("London123")]
    [InlineData("City 42")]
    [InlineData("Area51")]
    public async Task Validate_WithLocationContainingNumbers_IsValid(string location)
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = location 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region IncludeDetails Validation Tests

    [Fact]
    public async Task Validate_WithIncludeDetailsTrue_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            IncludeDetails = true 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithIncludeDetailsFalse_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            IncludeDetails = false 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithDefaultIncludeDetails_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest { Days = 5 };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
        request.IncludeDetails.Should().BeFalse();
    }

    #endregion

    #region Combined Validation Tests

    [Fact]
    public async Task Validate_WithAllValidFields_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 10, 
            Location = "London", 
            IncludeDetails = true 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithMultipleInvalidFields_HasMultipleValidationErrors()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 0, 
            Location = "London@Invalid!" 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Days);
        result.ShouldHaveValidationErrorFor(x => x.Location);
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task Validate_WithInvalidDaysAndValidLocation_HasOnlyDaysError()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 50, 
            Location = "London" 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Days);
        result.ShouldNotHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public async Task Validate_WithValidDaysAndInvalidLocation_HasOnlyLocationError()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 10, 
            Location = "London@!" 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Days);
        result.ShouldHaveValidationErrorFor(x => x.Location);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Validate_WithBoundaryDays_BothBoundariesValid()
    {
        // Arrange
        var requestMin = new GetWeatherForecastRequest 
        { 
            Days = GetWeatherForecastRequestValidator.MinDays 
        };
        var requestMax = new GetWeatherForecastRequest 
        { 
            Days = GetWeatherForecastRequestValidator.MaxDays 
        };

        // Act
        var resultMin = await _validator.TestValidateAsync(requestMin);
        var resultMax = await _validator.TestValidateAsync(requestMax);

        // Assert
        resultMin.ShouldNotHaveAnyValidationErrors();
        resultMax.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithSingleCharacterLocation_IsValid()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = "A" 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithUnicodeCharactersInLocation_HasValidationError()
    {
        // Arrange
        var request = new GetWeatherForecastRequest 
        { 
            Days = 5, 
            Location = "London™" 
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorCode("WF_LOCATION_INVALID");
    }

    #endregion

    #region Validator Configuration Tests

    [Fact]
    public void Validator_Constants_HaveExpectedValues()
    {
        // Assert
        GetWeatherForecastRequestValidator.MinDays.Should().Be(1);
        GetWeatherForecastRequestValidator.MaxDays.Should().Be(30);
        GetWeatherForecastRequestValidator.MaxLocationLength.Should().Be(100);
    }

    [Fact]
    public void Validator_CanBeInstantiated()
    {
        // Act
        var validator = new GetWeatherForecastRequestValidator();

        // Assert
        validator.Should().NotBeNull();
        validator.Should().BeAssignableTo<AbstractValidator<GetWeatherForecastRequest>>();
    }

    #endregion
}
