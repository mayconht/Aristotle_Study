using System.Runtime.Serialization;

namespace Aristotle.Application.Exceptions;

/// <summary>
/// ServiceOperationException is thrown when a specific service operation fails.
/// It provides additional context about the operation that failed, including the service name and operation details
/// </summary>
[Serializable]
public class ServiceOperationException : ApplicationException
{
    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// Gets additional context about the failure.
    /// </summary>
    public Dictionary<string, object> Context { get; }

    /// <summary>
    /// Initializes a new instance of the ServiceOperationException class.
    /// </summary>
    public ServiceOperationException() : base("A service operation failed.")
    {
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the ServiceOperationException class with a custom message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ServiceOperationException(string message) : base(message)
    {
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the ServiceOperationException class with service and operation details.
    /// </summary>
    /// <param name="service">The service that caused this exception.</param>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ServiceOperationException(string service, string operation, string message) : base(service, message)
    {
        Operation = operation;
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the ServiceOperationException class with context information.
    /// </summary>
    /// <param name="service">The service that caused this exception.</param>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="context">Additional context about the failure.</param>
    public ServiceOperationException(string service, string operation, string message,
        Dictionary<string, object> context)
        : base(service, message)
    {
        Operation = operation;
        Context = context;
    }

    /// <summary>
    /// Initializes a new instance of the ServiceOperationException class with an inner exception.
    /// </summary>
    /// <param name="service">The service that caused this exception.</param>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceOperationException(string service, string operation, string message, Exception innerException)
        : base(service, message, innerException)
    {
        Operation = operation;
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the ServiceOperationException class with serialized data.
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
    /// <param name="context">The StreamingContext that contains contextual information.</param>
    protected ServiceOperationException(SerializationInfo info, StreamingContext context) : base(
        "A service operation failed.")
    {
        Operation = info.GetString(nameof(Operation));
        Context = (Dictionary<string, object>?)info.GetValue(nameof(Context),
            typeof(Dictionary<string, object>)) ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// Adds context information to the exception.
    /// </summary>
    /// <param name="key">The context key.</param>
    /// <param name="value">The context value.</param>
    public void AddContext(string key, object value)
    {
        Context[key] = value;
    }
}