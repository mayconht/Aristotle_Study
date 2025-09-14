using Aristotle.Domain.Entities;

namespace Aristotle.Domain.Interfaces;

// I truly like to keep interfaces lke this, it is easier to find the data access layer
// and it is eeasier to implement the repository pattern
/// <summary>
/// 
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Gets a user by their email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Deletes a user by their id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Wipes all data from the user database. Primarily used for testing purposes.
    /// </summary>
    /// <returns></returns>
    Task WipeDatabaseAsync();
}