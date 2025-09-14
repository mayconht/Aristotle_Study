namespace Aristotle.Application.Exceptions;

/// <summary>
/// ServiceOperationException is thrown when a specific service operation fails.
/// It provides additional context about the operation that failed, including the service name and operation details
/// </summary>
public class ServiceOperationException : ApplicationException
{
    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public string? Operation { get; }


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
    }
}