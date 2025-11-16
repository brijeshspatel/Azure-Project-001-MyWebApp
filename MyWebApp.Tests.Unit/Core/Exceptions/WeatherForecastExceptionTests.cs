using FluentAssertions;
using MyWebApp.Core.Exceptions;
using Xunit;

namespace MyWebApp.Tests.Unit.Core.Exceptions;

/// <summary>
/// Tests for <see cref="WeatherForecastException"/>.
/// </summary>
public class WeatherForecastExceptionTests
{
    [Fact]
    public void Constructor_WithoutParameters_CreatesExceptionWithDefaultMessage()
    {
        // Act
        var exception = new WeatherForecastException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("weather forecast");
        exception.ErrorCode.Should().Be("WF000");
    }

    [Fact]
    public void Constructor_WithMessage_CreatesExceptionWithSpecifiedMessage()
    {
        // Arrange
        const string expectedMessage = "Test error message";

        // Act
        var exception = new WeatherForecastException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be("WF000");
    }

    [Fact]
    public void Constructor_WithMessageAndErrorCode_CreatesExceptionWithBothProperties()
    {
        // Arrange
        const string expectedMessage = "Test error message";
        const string expectedErrorCode = "WF123";

        // Act
        var exception = new WeatherForecastException(expectedMessage, expectedErrorCode);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be(expectedErrorCode);
    }

    [Fact]
    public void Constructor_WithMessageErrorCodeAndDays_CreatesExceptionWithAllProperties()
    {
        // Arrange
        const string expectedMessage = "Test error message";
        const string expectedErrorCode = "WF123";
        const int expectedDays = 50;

        // Act
        var exception = new WeatherForecastException(expectedMessage, expectedErrorCode, expectedDays);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be(expectedErrorCode);
        exception.RequestedDays.Should().Be(expectedDays);
    }

    [Fact]
    public void Constructor_WithInnerException_CreatesExceptionWithInnerException()
    {
        // Arrange
        const string expectedMessage = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new WeatherForecastException(expectedMessage, innerException);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.InnerException.Should().Be(innerException);
        exception.ErrorCode.Should().Be("WF000");
    }

    [Fact]
    public void InvalidDayRange_CreatesExceptionWithCorrectProperties()
    {
        // Arrange
        const int days = 50;
        const int minDays = 1;
        const int maxDays = 30;

        // Act
        var exception = WeatherForecastException.InvalidDayRange(days, minDays, maxDays);

        // Assert
        exception.Should().NotBeNull();
        exception.ErrorCode.Should().Be("WF001");
        exception.RequestedDays.Should().Be(days);
        exception.Message.Should().Contain(days.ToString());
        exception.Message.Should().Contain(minDays.ToString());
        exception.Message.Should().Contain(maxDays.ToString());
    }

    [Theory]
    [InlineData(0, 1, 30)]
    [InlineData(-5, 1, 30)]
    [InlineData(31, 1, 30)]
    [InlineData(100, 1, 30)]
    public void InvalidDayRange_WithDifferentValues_CreatesExceptionWithCorrectDays(
        int days,
        int minDays,
        int maxDays)
    {
        // Act
        var exception = WeatherForecastException.InvalidDayRange(days, minDays, maxDays);

        // Assert
        exception.RequestedDays.Should().Be(days);
        exception.ErrorCode.Should().Be("WF001");
    }

    [Fact]
    public void ForecastUnavailable_CreatesExceptionWithCorrectProperties()
    {
        // Arrange
        const string reason = "Service temporarily unavailable";

        // Act
        var exception = WeatherForecastException.ForecastUnavailable(reason);

        // Assert
        exception.Should().NotBeNull();
        exception.ErrorCode.Should().Be("WF002");
        exception.Message.Should().Contain(reason);
        exception.Message.Should().Contain("unavailable");
    }

    [Theory]
    [InlineData("Database connection failed")]
    [InlineData("External API timeout")]
    [InlineData("Rate limit exceeded")]
    public void ForecastUnavailable_WithDifferentReasons_CreatesExceptionWithCorrectMessage(string reason)
    {
        // Act
        var exception = WeatherForecastException.ForecastUnavailable(reason);

        // Assert
        exception.Message.Should().Contain(reason);
        exception.ErrorCode.Should().Be("WF002");
    }

    [Fact]
    public void WeatherForecastException_InheritFromDomainException()
    {
        // Arrange & Act
        var exception = new WeatherForecastException();

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void WeatherForecastException_InheritFromException()
    {
        // Arrange & Act
        var exception = new WeatherForecastException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
