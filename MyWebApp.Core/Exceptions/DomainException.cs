using System.Runtime.Serialization;

namespace MyWebApp.Core.Exceptions;

/// <summary>
/// Base exception class for all domain-specific exceptions.
/// </summary>
[Serializable]
public class DomainException : Exception
{
    /// <summary>
    /// Gets the error code associated with this exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    public DomainException()
        : base("A domain exception occurred.")
    {
        ErrorCode = "DOM000";
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DomainException(string message)
        : base(message)
    {
        ErrorCode = "DOM000";
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DomainException"/> class with a specified error message and error code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DomainException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = "DOM000";
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DomainException"/> class with a specified error message, error code, and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DomainException(string message, string errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="DomainException"/> class with serialised data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialised object data.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information.</param>
    [Obsolete("This constructor is obsolete due to BinaryFormatter deprecation.")]
    protected DomainException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        ErrorCode = info.GetString(nameof(ErrorCode)) ?? "DOM000";
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
        info.AddValue(nameof(ErrorCode), ErrorCode);
    }
}
