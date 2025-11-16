using FluentAssertions;
using MyWebApp.Core.Exceptions;
using Xunit;

namespace MyWebApp.Tests.Unit.Core.Exceptions;

/// <summary>
/// Tests for <see cref="NotFoundException"/>.
/// </summary>
public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithoutParameters_CreatesExceptionWithDefaultValues()
    {
        // Act
        var exception = new NotFoundException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("not found");
        exception.ErrorCode.Should().Be("NF000");
        exception.EntityType.Should().Be("Unknown");
        exception.EntityId.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithMessage_CreatesExceptionWithSpecifiedMessage()
    {
        // Arrange
        const string expectedMessage = "Weather forecast data not found";

        // Act
        var exception = new NotFoundException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.ErrorCode.Should().Be("NF000");
        exception.EntityType.Should().Be("Unknown");
    }

    [Fact]
    public void Constructor_WithEntityTypeAndId_CreatesExceptionWithBothProperties()
    {
        // Arrange
        const string entityType = "WeatherForecast";
        const int entityId = 123;

        // Act
        var exception = new NotFoundException(entityType, entityId);

        // Assert
        exception.EntityType.Should().Be(entityType);
        exception.EntityId.Should().Be(entityId);
        exception.ErrorCode.Should().Be("NF001");
        exception.Message.Should().Contain(entityType);
        exception.Message.Should().Contain(entityId.ToString());
    }

    [Fact]
    public void Constructor_WithEntityTypeIdAndMessage_CreatesExceptionWithAllProperties()
    {
        // Arrange
        const string entityType = "ForecastLocation";
        const string entityId = "LOC-LONDON";
        const string message = "The specified forecast location could not be found";

        // Act
        var exception = new NotFoundException(entityType, entityId, message);

        // Assert
        exception.EntityType.Should().Be(entityType);
        exception.EntityId.Should().Be(entityId);
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be("NF001");
    }

    [Fact]
    public void Constructor_WithInnerException_CreatesExceptionWithInnerException()
    {
        // Arrange
        const string message = "Weather data retrieval failed";
        var innerException = new InvalidOperationException("Forecast cache miss");

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.ErrorCode.Should().Be("NF000");
    }

    [Theory]
    [InlineData("WeatherForecast", 1)]
    [InlineData("ForecastLocation", 999)]
    [InlineData("ForecastSession", 12345)]
    public void Constructor_WithDifferentEntityTypes_CreatesCorrectException(string entityType, int entityId)
    {
        // Act
        var exception = new NotFoundException(entityType, entityId);

        // Assert
        exception.EntityType.Should().Be(entityType);
        exception.EntityId.Should().Be(entityId);
        exception.Message.Should().Contain(entityType);
    }

    [Theory]
    [InlineData("ForecastLocation", "LOC-001")]
    [InlineData("WeatherStation", "STATION-GB-001")]
    [InlineData("ForecastRequest", "REQ-2025-001")]
    public void Constructor_WithStringEntityIds_CreatesCorrectException(string entityType, string entityId)
    {
        // Act
        var exception = new NotFoundException(entityType, entityId);

        // Assert
        exception.EntityType.Should().Be(entityType);
        exception.EntityId.Should().Be(entityId);
        exception.Message.Should().Contain(entityId);
    }

    [Fact]
    public void Constructor_WithGuidEntityId_CreatesCorrectException()
    {
        // Arrange
        const string entityType = "ForecastSession";
        var entityId = Guid.NewGuid();

        // Act
        var exception = new NotFoundException(entityType, entityId);

        // Assert
        exception.EntityType.Should().Be(entityType);
        exception.EntityId.Should().Be(entityId);
        exception.Message.Should().Contain(entityId.ToString());
    }

    [Fact]
    public void EntityType_Property_IsReadable()
    {
        // Arrange
        const string expectedEntityType = "WeatherForecast";
        var exception = new NotFoundException(expectedEntityType, 123);

        // Act
        var actualEntityType = exception.EntityType;

        // Assert
        actualEntityType.Should().Be(expectedEntityType);
    }

    [Fact]
    public void EntityId_Property_IsReadable()
    {
        // Arrange
        const int expectedEntityId = 456;
        var exception = new NotFoundException("ForecastData", expectedEntityId);

        // Act
        var actualEntityId = exception.EntityId;

        // Assert
        actualEntityId.Should().Be(expectedEntityId);
    }

    [Fact]
    public void NotFoundException_InheritsFromDomainException()
    {
        // Arrange & Act
        var exception = new NotFoundException();

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void NotFoundException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new NotFoundException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void NotFoundException_IsSerializable()
    {
        // Arrange & Act
        var exception = new NotFoundException();
        var type = exception.GetType();

        // Assert
        type.GetCustomAttributes(typeof(SerializableAttribute), false)
            .Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithNullEntityType_DoesNotThrow()
    {
        // Act
        var act = () => new NotFoundException(null!, 123);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithEmptyEntityType_DoesNotThrow()
    {
        // Act
        var act = () => new NotFoundException(string.Empty, 123);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_WithNullEntityId_DoesNotThrow()
    {
        // Act
        var act = () => new NotFoundException("WeatherForecast", null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Message_ContainsBothEntityTypeAndId_WhenBothProvided()
    {
        // Arrange
        const string entityType = "ForecastLocation";
        const int entityId = 789;

        // Act
        var exception = new NotFoundException(entityType, entityId);

        // Assert
        exception.Message.Should().Contain(entityType);
        exception.Message.Should().Contain(entityId.ToString());
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public void ErrorCode_IsNF001_WhenEntityTypeAndIdProvided()
    {
        // Arrange & Act
        var exception = new NotFoundException("WeatherForecast", 123);

        // Assert
        exception.ErrorCode.Should().Be("NF001");
    }

    [Fact]
    public void ErrorCode_IsNF000_WhenOnlyMessageProvided()
    {
        // Arrange & Act
        var exception = new NotFoundException("Weather forecast not found error");

        // Assert
        exception.ErrorCode.Should().Be("NF000");
    }

    [Fact]
    public void InnerException_IsPreserved_WhenProvided()
    {
        // Arrange
        var originalException = new KeyNotFoundException("Forecast key not found");
        var notFoundException = new NotFoundException("Weather forecast not found", originalException);

        // Act & Assert
        notFoundException.InnerException.Should().BeSameAs(originalException);
    }

    [Fact]
    public void Constructor_WithDateOnlyAsEntityId_StoresCorrectly()
    {
        // Arrange
        var forecastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        const string entityType = "ForecastByDate";

        // Act
        var exception = new NotFoundException(entityType, forecastDate);

        // Assert
        exception.EntityId.Should().Be(forecastDate);
        exception.EntityType.Should().Be(entityType);
    }

    [Fact]
    public void Constructor_WithLocationCodeAsEntityId_CreatesValidException()
    {
        // Arrange
        const string locationCode = "GB-LON";

        // Act
        var exception = new NotFoundException("WeatherLocation", locationCode);

        // Assert
        exception.EntityId.Should().Be(locationCode);
        exception.EntityType.Should().Be("WeatherLocation");
        exception.Message.Should().Contain(locationCode);
    }
}
