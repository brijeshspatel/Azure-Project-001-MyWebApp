using System.Runtime.Serialization;

namespace MyWebApp.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
[Serializable]
public class NotFoundException : DomainException
{
    /// <summary>
    /// Gets the type of the entity that was not found.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity that was not found.
    /// </summary>
    public object EntityId { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException()
        : base("The requested resource was not found.", "NF000")
    {
        EntityType = "Unknown";
        EntityId = string.Empty;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message)
        : base(message, "NF000")
    {
        EntityType = "Unknown";
        EntityId = string.Empty;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NotFoundException"/> class with entity type and identifier.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="entityId">The identifier of the entity that was not found.</param>
    public NotFoundException(string entityType, object entityId)
        : base($"{entityType} with identifier '{entityId}' was not found.", "NF001")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NotFoundException"/> class with entity type, identifier, and custom message.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="entityId">The identifier of the entity that was not found.</param>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string entityType, object entityId, string message)
        : base(message, "NF001")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NotFoundException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, Exception innerException)
        : base(message, "NF000", innerException)
    {
        EntityType = "Unknown";
        EntityId = string.Empty;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="NotFoundException"/> class with serialised data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialised object data.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information.</param>
    [Obsolete("This constructor is obsolete due to BinaryFormatter deprecation.")]
    protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        EntityType = info.GetString(nameof(EntityType)) ?? "Unknown";
        EntityId = info.GetValue(nameof(EntityId), typeof(object)) ?? string.Empty;
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
        info.AddValue(nameof(EntityType), EntityType);
        info.AddValue(nameof(EntityId), EntityId);
    }
}
