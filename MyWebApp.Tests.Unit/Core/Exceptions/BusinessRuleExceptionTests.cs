using FluentAssertions;
using MyWebApp.Core.Exceptions;
using Xunit;

namespace MyWebApp.Tests.Unit.Core.Exceptions;

/// <summary>
/// Tests for <see cref="BusinessRuleException"/>.
/// </summary>
public class BusinessRuleExceptionTests
{
    [Fact]
    public void Constructor_WithoutParameters_CreatesExceptionWithDefaultValues()
    {
        // Act
        var exception = new BusinessRuleException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("business rule");
        exception.ErrorCode.Should().Be("BR000");
        exception.RuleName.Should().Be("Unknown");
    }

    [Fact]
    public void Constructor_WithMessage_CreatesExceptionWithSpecifiedMessage()
    {
        // Arrange
        const string expectedMessage = "Weather forecast business rule violation occurred";

        // Act
        var exception = new BusinessRuleException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be("BR000");
        exception.RuleName.Should().Be("Unknown");
    }

    [Fact]
    public void Constructor_WithRuleNameAndMessage_CreatesExceptionWithBothProperties()
    {
        // Arrange
        const string ruleName = "MaximumForecastDays";
        const string message = "Forecast cannot exceed 30 days";

        // Act
        var exception = new BusinessRuleException(ruleName, message);

        // Assert
        exception.RuleName.Should().Be(ruleName);
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("BR001");
    }

    [Fact]
    public void Constructor_WithRuleNameMessageAndErrorCode_CreatesExceptionWithAllProperties()
    {
        // Arrange
        const string ruleName = "MinimumForecastDays";
        const string message = "Forecast must be at least 1 day";
        const string errorCode = "BR_FORECAST_MIN";

        // Act
        var exception = new BusinessRuleException(ruleName, message, errorCode);

        // Assert
        exception.RuleName.Should().Be(ruleName);
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void Constructor_WithInnerException_CreatesExceptionWithInnerException()
    {
        // Arrange
        const string message = "Weather forecast rule check failed";
        var innerException = new InvalidOperationException("Temperature data unavailable");

        // Act
        var exception = new BusinessRuleException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.ErrorCode.Should().Be("BR000");
        exception.RuleName.Should().Be("Unknown");
    }

    [Fact]
    public void Constructor_WithRuleNameMessageAndInnerException_CreatesExceptionWithAllProperties()
    {
        // Arrange
        const string ruleName = "ForecastDataAvailability";
        const string message = "Weather data not available for requested period";
        var innerException = new InvalidOperationException("Historical data missing");

        // Act
        var exception = new BusinessRuleException(ruleName, message, innerException);

        // Assert
        exception.RuleName.Should().Be(ruleName);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.ErrorCode.Should().Be("BR001");
    }

    [Theory]
    [InlineData("ForecastRangeLimitation", "Forecast range exceeds system capabilities")]
    [InlineData("LocationValidation", "Invalid location for weather forecast")]
    [InlineData("DateRangeValidation", "Forecast dates must be in the future")]
    public void Constructor_WithDifferentRules_CreatesExceptionWithCorrectProperties(
        string ruleName,
        string message)
    {
        // Act
        var exception = new BusinessRuleException(ruleName, message);

        // Assert
        exception.RuleName.Should().Be(ruleName);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void RuleName_Property_IsReadable()
    {
        // Arrange
        const string expectedRuleName = "WeatherDataIntegrity";
        var exception = new BusinessRuleException(expectedRuleName, "Weather data integrity check failed");

        // Act
        var actualRuleName = exception.RuleName;

        // Assert
        actualRuleName.Should().Be(expectedRuleName);
    }

    [Fact]
    public void BusinessRuleException_InheritsFromDomainException()
    {
        // Arrange & Act
        var exception = new BusinessRuleException();

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void BusinessRuleException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new BusinessRuleException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void BusinessRuleException_IsSerializable()
    {
        // Arrange & Act
        var exception = new BusinessRuleException();
        var type = exception.GetType();

        // Assert
        type.GetCustomAttributes(typeof(SerializableAttribute), false)
            .Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("BR_FORECAST_001")]
    [InlineData("BR_LOCATION_002")]
    [InlineData("BR_DATE_RANGE")]
    public void Constructor_WithCustomErrorCodes_CreatesExceptionWithCorrectErrorCode(string errorCode)
    {
        // Arrange
        const string ruleName = "WeatherForecastRule";
        const string message = "Weather forecast business rule violated";

        // Act
        var exception = new BusinessRuleException(ruleName, message, errorCode);

        // Assert
        exception.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void Constructor_WithNullRuleName_DoesNotThrow()
    {
        // Act
        var act = () => new BusinessRuleException(null!, "Forecast error");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithEmptyRuleName_DoesNotThrow()
    {
        // Act
        var act = () => new BusinessRuleException(string.Empty, "Forecast error");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithNullMessage_DoesNotThrow()
    {
        // Act
        var act = () => new BusinessRuleException("ForecastRule", (string)null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void InnerException_IsPreserved_WhenProvided()
    {
        // Arrange
        var originalException = new ArgumentException("Invalid temperature range");
        var businessException = new BusinessRuleException("TemperatureValidation", "Temperature out of range", originalException);

        // Act & Assert
        businessException.InnerException.Should().BeSameAs(originalException);
    }

    [Fact]
    public void ErrorCode_DefaultsToBR001_WhenRuleNameProvided()
    {
        // Arrange & Act
        var exception = new BusinessRuleException("ForecastLimit", "Forecast limit exceeded");

        // Assert
        exception.ErrorCode.Should().Be("BR001");
    }

    [Fact]
    public void ErrorCode_DefaultsToBR000_WhenRuleNameNotProvided()
    {
        // Arrange & Act
        var exception = new BusinessRuleException("Weather forecast error");

        // Assert
        exception.ErrorCode.Should().Be("BR000");
    }

    [Fact]
    public void Constructor_WithAllParameters_PreservesAllValues()
    {
        // Arrange
        const string ruleName = "ForecastDataCompleteness";
        const string message = "Weather forecast data incomplete";
        var innerException = new InvalidOperationException("Temperature data missing");

        // Act
        var exception = new BusinessRuleException(ruleName, message, innerException);

        // Assert
        exception.RuleName.Should().Be(ruleName);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.ErrorCode.Should().Be("BR001");
    }
}
