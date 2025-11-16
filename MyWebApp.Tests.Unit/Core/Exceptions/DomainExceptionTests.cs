using FluentAssertions;
using MyWebApp.Core.Exceptions;
using Xunit;

namespace MyWebApp.Tests.Unit.Core.Exceptions;

/// <summary>
/// Tests for <see cref="DomainException"/>.
/// </summary>
public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithoutParameters_CreatesExceptionWithDefaultValues()
    {
        // Act
        var exception = new DomainException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("domain exception");
        exception.ErrorCode.Should().Be("DOM000");
    }

    [Fact]
    public void Constructor_WithMessage_CreatesExceptionWithSpecifiedMessage()
    {
        // Arrange
        const string expectedMessage = "Weather service encountered an error";

        // Act
        var exception = new DomainException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be("DOM000");
    }

    [Fact]
    public void Constructor_WithMessageAndErrorCode_CreatesExceptionWithBothProperties()
    {
        // Arrange
        const string expectedMessage = "Weather data validation failed";
        const string expectedErrorCode = "DOM_WF001";

        // Act
        var exception = new DomainException(expectedMessage, expectedErrorCode);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be(expectedErrorCode);
    }

    [Fact]
    public void Constructor_WithInnerException_CreatesExceptionWithInnerException()
    {
        // Arrange
        const string expectedMessage = "Weather forecast generation failed";
        var innerException = new InvalidOperationException("Forecast calculation error");

        // Act
        var exception = new DomainException(expectedMessage, innerException);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Message.Should().Be("Forecast calculation error");
        exception.ErrorCode.Should().Be("DOM000");
    }

    [Fact]
    public void Constructor_WithMessageErrorCodeAndInnerException_CreatesExceptionWithAllProperties()
    {
        // Arrange
        const string expectedMessage = "Weather data processing error";
        const string expectedErrorCode = "DOM_PROC001";
        var innerException = new ArgumentException("Invalid temperature data");

        // Act
        var exception = new DomainException(expectedMessage, expectedErrorCode, innerException);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be(expectedErrorCode);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [InlineData("DOM_WF001", "Weather forecast error")]
    [InlineData("DOM_VAL001", "Weather data validation error")]
    [InlineData("DOM_CALC001", "Temperature calculation error")]
    public void Constructor_WithDifferentErrorCodes_CreatesExceptionWithCorrectErrorCode(
        string errorCode,
        string message)
    {
        // Act
        var exception = new DomainException(message, errorCode);

        // Assert
        exception.ErrorCode.Should().Be(errorCode);
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DomainException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new DomainException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void DomainException_IsSerializable()
    {
        // Arrange & Act
        var exception = new DomainException();
        var type = exception.GetType();

        // Assert
        type.GetCustomAttributes(typeof(SerializableAttribute), false)
            .Should().NotBeEmpty();
    }

    [Fact]
    public void ErrorCode_Property_IsReadable()
    {
        // Arrange
        const string expectedErrorCode = "DOM_FORECAST_TEST";
        var exception = new DomainException("Forecast test error", expectedErrorCode);

        // Act
        var actualErrorCode = exception.ErrorCode;

        // Assert
        actualErrorCode.Should().Be(expectedErrorCode);
    }

    [Fact]
    public void InnerException_IsPreserved_WhenProvided()
    {
        // Arrange
        var originalException = new InvalidOperationException("Temperature sensor malfunction");
        var domainException = new DomainException("Weather reading failed", originalException);

        // Act & Assert
        domainException.InnerException.Should().BeSameAs(originalException);
    }

    [Fact]
    public void Constructor_WithNullMessage_DoesNotThrow()
    {
        // Act
        var act = () => new DomainException(null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithEmptyMessage_DoesNotThrow()
    {
        // Act
        var act = () => new DomainException(string.Empty);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithNullErrorCode_InConstructor_DoesNotThrow()
    {
        // Act
        var act = () => new DomainException("Forecast error", (string)null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void StackTrace_IsPreserved_WhenThrown()
    {
        // Arrange
        DomainException? caughtException = null;

        // Act
        try
        {
            throw new DomainException("Weather forecast system error");
        }
        catch (DomainException ex)
        {
            caughtException = ex;
        }

        // Assert
        caughtException.Should().NotBeNull();
        caughtException!.StackTrace.Should().NotBeNullOrWhiteSpace();
    }
}
