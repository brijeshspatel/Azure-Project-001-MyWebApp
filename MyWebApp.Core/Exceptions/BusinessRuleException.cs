using System.Runtime.Serialization;

namespace MyWebApp.Core.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
[Serializable]
public class BusinessRuleException : DomainException
{
    /// <summary>
    /// Gets the name of the business rule that was violated.
    /// </summary>
    public string RuleName { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class.
    /// </summary>
    public BusinessRuleException()
        : base("A business rule was violated.", "BR000")
    {
        RuleName = "Unknown";
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BusinessRuleException(string message)
        : base(message, "BR000")
    {
        RuleName = "Unknown";
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class with a rule name and error message.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    /// <param name="message">The message that describes the error.</param>
    public BusinessRuleException(string ruleName, string message)
        : base(message, "BR001")
    {
        RuleName = ruleName;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class with a rule name, error message, and error code.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The error code associated with this exception.</param>
    public BusinessRuleException(string ruleName, string message, string errorCode)
        : base(message, errorCode)
    {
        RuleName = ruleName;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessRuleException(string message, Exception innerException)
        : base(message, "BR000", innerException)
    {
        RuleName = "Unknown";
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class with a rule name, error message, and inner exception.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessRuleException(string ruleName, string message, Exception innerException)
        : base(message, "BR001", innerException)
    {
        RuleName = ruleName;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="BusinessRuleException"/> class with serialised data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialised object data.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information.</param>
    [Obsolete("This constructor is obsolete due to BinaryFormatter deprecation.")]
    protected BusinessRuleException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        RuleName = info.GetString(nameof(RuleName)) ?? "Unknown";
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
        info.AddValue(nameof(RuleName), RuleName);
    }
}
