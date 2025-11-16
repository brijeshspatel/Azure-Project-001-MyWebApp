using System.Runtime.Serialization;

namespace MyWebApp.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails for a domain entity or value object.
/// </summary>
[Serializable]
public class ValidationException : DomainException
{
    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException()
        : base("One or more validation errors occurred.", "VAL000")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValidationException(string message)
        : base(message, "VAL000")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ValidationException"/> class with validation errors.
    /// </summary>
    /// <param name="errors">The dictionary of validation errors.</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", "VAL000")
    {
        Errors = errors;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ValidationException"/> class with a field name and error message.
    /// </summary>
    /// <param name="fieldName">The name of the field that failed validation.</param>
    /// <param name="errorMessage">The validation error message.</param>
    public ValidationException(string fieldName, string errorMessage)
        : base($"Validation failed for field '{fieldName}': {errorMessage}", "VAL001")
    {
        Errors = new Dictionary<string, string[]>
        {
            [fieldName] = new[] { errorMessage }
        };
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ValidationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ValidationException(string message, Exception innerException)
        : base(message, "VAL000", innerException)
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ValidationException"/> class with serialised data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialised object data.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information.</param>
    [Obsolete("This constructor is obsolete due to BinaryFormatter deprecation.")]
    protected ValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Errors = (IDictionary<string, string[]>?)info.GetValue(nameof(Errors), typeof(IDictionary<string, string[]>))
                 ?? new Dictionary<string, string[]>();
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
        info.AddValue(nameof(Errors), Errors);
    }
}
