namespace Aristotle.Domain.Exceptions;

//The Error class is widely known so you can find examples of it in many places.
/// <summary>
/// Base exception class for domain-related exceptions.
/// This class represents violations of business rules or domain constraints.
/// </summary>
public abstract class ADomainException : Exception
{
    /// <summary>
    /// Gets the error code associated with this domain exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the DomainException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected ADomainException(string message) : base(message)
    {
        ErrorCode = GetType().Name;
    }
}