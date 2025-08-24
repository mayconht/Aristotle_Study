namespace Aristotle.Domain.Exceptions;

/// <summary>
/// Exception thrown when a user-related operation fails due to business rule violations.
/// This exception encapsulates common user domain scenarios such as duplicate emails,
/// invalid user data, or user state violations.
/// </summary>
public class UserDomainException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the UserDomainException class with user email.
    /// </summary>
    /// <param name="userEmail">The user email associated with this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected UserDomainException(string userEmail, string message) : base(message)
    {
        // UserEmail parameter is used for the message but not stored since it's not accessed anywhere
    }
}

/// <summary>
/// Exception thrown when attempting to create a user with an email that already exists.
/// This represents a violation of the unique email constraint in the user domain.
/// </summary>
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
}

/// <summary>
/// Exception thrown when a user is not found by the specified criteria.
/// This is a specialized version of EntityNotFoundException for user entities.
/// </summary>
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
}