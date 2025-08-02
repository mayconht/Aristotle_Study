namespace Aristotle.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user-related operation fails due to business rule violations.
/// This exception encapsulates common user domain scenarios such as duplicate emails,
/// invalid user data, or user state violations.
/// </summary>
[Serializable]
public class UserDomainException : DomainException
{
    /// <summary>
    /// Gets the user identifier associated with this exception.
    /// </summary>
    public Guid? UserId { get; }

    /// <summary>
    /// Gets the user email associated with this exception.
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Initializes a new instance of the UserDomainException class.
    /// </summary>
    public UserDomainException() : base("A user domain exception occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserDomainException class with a custom message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UserDomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserDomainException class with user ID.
    /// </summary>
    /// <param name="userId">The user identifier associated with this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UserDomainException(Guid userId, string message) : base(message)
    {
        UserId = userId;
    }

    /// <summary>
    /// Initializes a new instance of the UserDomainException class with user email.
    /// </summary>
    /// <param name="userEmail">The user email associated with this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UserDomainException(string userEmail, string message) : base(message)
    {
        UserEmail = userEmail;
    }

    /// <summary>
    /// Initializes a new instance of the UserDomainException class with user ID and email.
    /// </summary>
    /// <param name="userId">The user identifier associated with this exception.</param>
    /// <param name="userEmail">The user email associated with this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UserDomainException(Guid userId, string userEmail, string message) : base(message)
    {
        UserId = userId;
        UserEmail = userEmail;
    }

    /// <summary>
    /// Initializes a new instance of the UserDomainException class with an inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UserDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when attempting to create a user with an email that already exists.
/// This represents a violation of the unique email constraint in the user domain.
/// </summary>
[Serializable]
public class DuplicateUserEmailException : UserDomainException
{
    /// <summary>
    /// Initializes a new instance of the DuplicateUserEmailException class.
    /// </summary>
    /// <param name="email">The email that already exists.</param>
    public DuplicateUserEmailException(string email)
        : base(email, $"A user with email '{email}' already exists.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the DuplicateUserEmailException class with an inner exception.
    /// </summary>
    /// <param name="email">The email that already exists.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DuplicateUserEmailException(string email, Exception innerException)
        : base($"A user with email '{email}' already exists.", innerException)
    {
        UserEmail = email;
    }
}

/// <summary>
/// Exception thrown when a user is not found by the specified criteria.
/// This is a specialized version of EntityNotFoundException for user entities.
/// </summary>
[Serializable]
public class UserNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the UserNotFoundException class with a user ID.
    /// </summary>
    /// <param name="userId">The user ID that was not found.</param>
    public UserNotFoundException(Guid userId)
        : base("User", userId)
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserNotFoundException class with a user email.
    /// </summary>
    /// <param name="userEmail">The user email that was not found.</param>
    public UserNotFoundException(string userEmail)
        : base("User", userEmail)
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserNotFoundException class with a custom message.
    /// </summary>
    /// <param name="userId">The user ID that was not found.</param>
    /// <param name="message">Custom error message.</param>
    public UserNotFoundException(Guid userId, string message)
        : base("User", userId, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the UserNotFoundException class with an inner exception.
    /// </summary>
    /// <param name="userId">The user ID that was not found.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UserNotFoundException(Guid userId, Exception innerException)
        : base("User", userId, innerException)
    {
    }
}