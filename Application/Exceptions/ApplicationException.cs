namespace Aristotle.Application.Exceptions;

/// <summary>
/// Base exception class for application service layer exceptions.
/// </summary>
[Serializable]
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Gets the error code associated with this application exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the service or operation that caused this exception.
    /// </summary>
    public string? Service { get; }

    /// <summary>
    /// Initializes a new instance of the ApplicationException class.
    /// </summary>
    protected ApplicationException()
    {
        ErrorCode = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the ApplicationException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected ApplicationException(string message) : base(message)
    {
        ErrorCode = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the ApplicationException class with service information.
    /// </summary>
    /// <param name="service">The service or operation that caused this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected ApplicationException(string service, string message) : base(message)
    {
        ErrorCode = GetType().Name;
        Service = service;
    }

    /// <summary>
    /// Initializes a new instance of the ApplicationException class with an inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ApplicationException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the ApplicationException class with service and inner exception.
    /// </summary>
    /// <param name="service">The service or operation that caused this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ApplicationException(string service, string message, Exception innerException) : base(message,
        innerException)
    {
        ErrorCode = GetType().Name;
        Service = service;
    }
}