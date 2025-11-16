using FluentAssertions;
using MyWebApp.Core.Exceptions;
using Xunit;

namespace MyWebApp.Tests.Unit.Core.Exceptions;

/// <summary>
/// Tests for <see cref="ValidationException"/>.
/// </summary>
public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithoutParameters_CreatesExceptionWithDefaultValues()
    {
        // Act
        var exception = new ValidationException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("validation");
        exception.ErrorCode.Should().Be("VAL000");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithMessage_CreatesExceptionWithSpecifiedMessage()
    {
        // Arrange
        const string expectedMessage = "Weather forecast request validation failed";

        // Act
        var exception = new ValidationException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be("VAL000");
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithErrors_CreatesExceptionWithErrorsDictionary()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            ["Days"] = new[] { "Days must be between 1 and 30", "Days cannot be zero" },
            ["Location"] = new[] { "Location name is too long" }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().HaveCount(2);
        exception.Errors["Days"].Should().BeEquivalentTo("Days must be between 1 and 30", "Days cannot be zero");
        exception.Errors["Location"].Should().BeEquivalentTo("Location name is too long");
    }

    [Fact]
    public void Constructor_WithFieldNameAndError_CreatesExceptionWithSingleFieldError()
    {
        // Arrange
        const string fieldName = "Days";
        const string errorMessage = "The number of days must be between 1 and 30";

        // Act
        var exception = new ValidationException(fieldName, errorMessage);

        // Assert
        exception.ErrorCode.Should().Be("VAL001");
        exception.Message.Should().Contain(fieldName);
        exception.Message.Should().Contain(errorMessage);
        exception.Errors.Should().ContainKey(fieldName);
        exception.Errors[fieldName].Should().ContainSingle()
            .Which.Should().Be(errorMessage);
    }

    [Fact]
    public void Constructor_WithInnerException_CreatesExceptionWithInnerException()
    {
        // Arrange
        const string message = "Weather forecast request validation error";
        var innerException = new ArgumentException("Invalid forecast parameter");

        // Act
        var exception = new ValidationException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.ErrorCode.Should().Be("VAL000");
    }

    [Fact]
    public void Errors_Property_IsReadable()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            ["Location"] = new[] { "Location format is invalid" }
        };
        var exception = new ValidationException(errors);

        // Act
        var retrievedErrors = exception.Errors;

        // Assert
        retrievedErrors.Should().BeSameAs(errors);
    }

    [Fact]
    public void ValidationException_InheritsFromDomainException()
    {
        // Arrange & Act
        var exception = new ValidationException();

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ValidationException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new ValidationException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void ValidationException_IsSerializable()
    {
        // Arrange & Act
        var exception = new ValidationException();
        var type = exception.GetType();

        // Assert
        type.GetCustomAttributes(typeof(SerializableAttribute), false)
            .Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Days", "Days must be greater than zero")]
    [InlineData("Location", "Location name cannot exceed 100 characters")]
    [InlineData("IncludeDetails", "IncludeDetails must be a boolean value")]
    public void Constructor_WithDifferentFieldErrors_CreatesCorrectException(
        string fieldName,
        string errorMessage)
    {
        // Act
        var exception = new ValidationException(fieldName, errorMessage);

        // Assert
        exception.Errors[fieldName].Should().Contain(errorMessage);
        exception.Message.Should().Contain(fieldName);
    }

    [Fact]
    public void Constructor_WithMultipleErrorsForDaysField_StoresAllErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            ["Days"] = new[] 
            { 
                "Days is required", 
                "Days must be at least 1",
                "Days cannot exceed 30" 
            }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Errors["Days"].Should().HaveCount(3);
        exception.Errors["Days"].Should().Contain("Days is required");
        exception.Errors["Days"].Should().Contain("Days must be at least 1");
        exception.Errors["Days"].Should().Contain("Days cannot exceed 30");
    }

    [Fact]
    public void Constructor_WithEmptyErrorsDictionary_CreatesExceptionWithEmptyErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>();

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNullErrorMessage_DoesNotThrow()
    {
        // Act
        var act = () => new ValidationException("Days", (string)null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithEmptyFieldName_DoesNotThrow()
    {
        // Act
        var act = () => new ValidationException(string.Empty, "Validation error");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Errors_ContainsMultipleForecastFields_AllFieldsAreAccessible()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            ["Days"] = new[] { "Days validation error" },
            ["Location"] = new[] { "Location validation error" },
            ["IncludeDetails"] = new[] { "IncludeDetails validation error" }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.Errors.Keys.Should().Contain(new[] { "Days", "Location", "IncludeDetails" });
    }

    [Fact]
    public void Message_ContainsFieldName_WhenCreatedWithFieldError()
    {
        // Arrange
        const string fieldName = "Location";
        const string errorMessage = "Location validation failed for weather forecast";

        // Act
        var exception = new ValidationException(fieldName, errorMessage);

        // Assert
        exception.Message.Should().Contain(fieldName);
        exception.Message.Should().Contain("Validation failed");
    }
}
