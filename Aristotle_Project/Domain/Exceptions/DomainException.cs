namespace Aristotle.Domain.Exceptions;

//The Error class is widely known so you can find examples of it in many places.
/// <summary>
/// Base exception class for domain-related exceptions.
/// This class represents violations of business rules or domain constraints.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Gets the error code associated with this domain exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// </summary>
    protected DomainException()
    {
        ErrorCode = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected DomainException(string message) : base(message)
    {
        ErrorCode = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a specified error message and error code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The specific error code for this exception.</param>
    protected DomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a specified error message,
    /// error code and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The specific error code for this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected DomainException(string message, string errorCode, Exception innerException) : base(message,
        innerException)
    {
        ErrorCode = errorCode;
    }
}