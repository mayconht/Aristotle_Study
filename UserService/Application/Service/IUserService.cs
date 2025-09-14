using Aristotle.Domain.Entities;

namespace Aristotle.Application.Service;

/// <summary>
/// Provides methods for user-related business operations such as retrieving, creating, updating, and deleting users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user if found; otherwise, null.</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <returns>An enumerable collection of users.</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>The created user.</returns>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>The updated user.</returns>
    Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>True if the user was deleted; otherwise, false.</returns>
    Task<bool> DeleteUserAsync(Guid id);

    /// <summary>
    /// Wipes all data from the user database. Primarily used for testing purposes.
    /// </summary>
    /// <returns></returns>
    Task WipeDatabaseAsync();
}