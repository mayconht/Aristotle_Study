namespace Aristotle.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a user with an email that already exists.
/// This represents a violation of the unique email constraint in the user domain.
/// </summary>
public class DuplicateUserEmailException : ADomainException
{
    /// <summary>
    /// Initializes a new instance of the DuplicateUserEmailException class.
    /// </summary>
    /// <param name="email">The email that already exists.</param>
    public DuplicateUserEmailException(string email)
        : base($"A user with email '{email}' already exists.")
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

/// <summary>
/// Exception thrown when a user is not found by the specified criteria.
/// This is a specialized version of EntityNotFoundException for user entities.
/// </summary>
public class UserEmailNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the UserNotFoundException class with a user ID.
    /// </summary>
    /// <param name="email">The user email that was not found.</param>
    public UserEmailNotFoundException(string email)
        : base("User", email)
    {
    }
}