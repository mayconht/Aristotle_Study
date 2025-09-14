using System.Net.Mail;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Interfaces;
using Aristotle.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aristotle.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for User entity data access operations.
/// Handles database interactions with proper exception handling and logging.
/// Implements the IUserRepository interface following hexagonal architecture principles.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The Entity Framework database context.</param>
    /// <param name="logger">Logger for repository operations and error tracking.</param>
    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The user if found, null otherwise.</returns>
    /// <exception cref="DatabaseException">Thrown when database operation fails.</exception>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Retrieving user with ID: {UserId}", id);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        _logger.LogDebug("Retrieving user with ID {UserId} completed. Found: {Found}", id, user != null);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving user with ID: {UserId}", id);
            throw new DatabaseException(nameof(GetByIdAsync), nameof(User), $"Failed to retrieve user with ID: {id}", ex.ToString());
        }
    }

    /// <summary>
    /// Gets all users from the database.
    /// </summary>
    /// <returns>A collection of all users in the system.</returns>
    /// <exception cref="DatabaseException">Thrown when database operation fails.</exception>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving all users");
            var users = await _context.Users.ToListAsync();
            _logger.LogDebug("Successfully retrieved {UserCount} users", users.Count);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving all users");
            throw new DatabaseException(nameof(GetAllAsync), nameof(User), "Failed to retrieve all users", ex.ToString());
        }
    }

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            var finalMail = new MailAddress(email);
            _logger.LogDebug("Retrieving user by email lookup initiated");
            var user = _context.Users.FirstOrDefaultAsync(u => u.Email == finalMail.Address);
            _logger.LogDebug("Email lookup query executed successfully");
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving user by email lookup");
            throw new DatabaseException(nameof(GetByEmailAsync), nameof(User), "Failed to retrieve user by email lookup", ex.ToString());
        }
    }

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <returns>The added user with generated ID.</returns>
    /// <exception cref="ArgumentNullException">Thrown when user is null.</exception>
    /// <exception cref="DatabaseException">Thrown when database operation fails.</exception>
    public async Task<User> AddAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null");
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added user with ID: {UserId}", user.Id);
            return user;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogDebug(ex, "Database constraint violation while adding user with ID: {UserId}", user.Id);
            if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                throw new DatabaseException(nameof(AddAsync), nameof(User), "A user with this email already exists in the database", ex.ToString());
            throw new DatabaseException(nameof(AddAsync), nameof(User), "Failed to add user to database", ex.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while adding user with ID: {UserId}", user.Id);
            throw new DatabaseException(nameof(AddAsync), nameof(User), "Unexpected error occurred while adding user", ex.ToString());
        }
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>The updated user entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when user is null.</exception>
    /// <exception cref="RepositoryException">Thrown when user to update is not found.</exception>
    /// <exception cref="DatabaseException">Thrown when database operation fails.</exception>
    public async Task<User> UpdateAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null");
        try
        {
            _logger.LogDebug("Updating user with ID: {UserId}", user.Id);
            var existingUser = await _context.Users.FindAsync(user.Id);
            //TODO: We validate if the user exists too much, maybe this should be handled at a higher level
            if (existingUser == null)
            {
                _logger.LogWarning("Attempted to update non-existent user with ID: {UserId}", user.Id);
                throw new RepositoryException(nameof(UserRepository), user.Id, $"User with ID {user.Id} not found for update");
            }
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Successfully updated user with ID: {UserId}", user.Id);
            return user;
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict while updating user with ID: {UserId}", user.Id);
            throw new DatabaseException(nameof(UpdateAsync), nameof(User), "The user was modified by another process", ex.ToString());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database constraint violation while updating user with ID: {UserId}", user.Id);
            if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                throw new DatabaseException(nameof(UpdateAsync), nameof(User), "A user with this email already exists in the database", ex.ToString());
            throw new DatabaseException(nameof(UpdateAsync), nameof(User), "Failed to update user in database", ex.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating user with ID: {UserId}", user.Id);
            throw new DatabaseException(nameof(UpdateAsync), nameof(User), "Unexpected error occurred while updating user", ex.ToString());
        }
    }

    /// <summary>
    /// Deletes a user from the database by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>True if the user was deleted successfully, false if the user was not found.</returns>
    /// <exception cref="DatabaseException">Thrown when database operation fails.</exception>
    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Deleting user with ID: {UserId}", id);
            var user = await GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogDebug("User with ID {UserId} not found for deletion", id);
                return false;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Successfully deleted user with ID: {UserId}", id);
            return true;
        }
        catch (Exception ex) when (ex is not DatabaseException)
        {
            _logger.LogError(ex, "Database error occurred while deleting user with ID: {UserId}", id);
            throw new DatabaseException(nameof(DeleteAsync), nameof(User), $"Failed to delete user with ID: {id}", ex.ToString());
        }
    }

    /// <summary>
    /// Wipes all data from the user database. Primarily used for testing purposes.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DatabaseException"></exception>
    public Task WipeDatabaseAsync()
    {
        try
        {
            _logger.LogWarning("Wiping all user data from the database. This operation is irreversible.");
            _context.Users.RemoveRange(_context.Users);
            return _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error occurred while wiping user database");
            throw new DatabaseException(nameof(WipeDatabaseAsync), nameof(User), "Failed to wipe user database", ex.ToString());
        }
    }
}