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
    private const string TableName = "Users";
    private const string UpdateOperation = "Update";
    private const string AddOperation = "Add";
    private const string DeleteOperation = "Delete";
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

            switch (user)
            {
                case null:
                    _logger.LogDebug("User with ID {UserId} not found", id);
                    break;
                default:
                    _logger.LogDebug("Successfully retrieved user with ID: {UserId}", id);
                    break;
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error occurred while retrieving user with ID: {UserId}", id);
            throw new DatabaseException("GetById", TableName, $"Failed to retrieve user with ID: {id}", ex.ToString());
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
            throw new DatabaseException("GetAll", TableName, "Failed to retrieve all users", ex.ToString());
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
            throw new DatabaseException("GetByEmail", TableName, "Failed to retrieve user by email lookup",
                ex.ToString());
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
                throw new DatabaseException(AddOperation, TableName,
                    "A user with this email already exists in the database",
                    ex.ToString());

            throw new DatabaseException(AddOperation, TableName, "Failed to add user to database", ex.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while adding user with ID: {UserId}", user.Id);
            throw new DatabaseException(AddOperation, TableName, "Unexpected error occurred while adding user",
                ex.ToString());
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
            if (existingUser == null)
            {
                _logger.LogWarning("Attempted to update non-existent user with ID: {UserId}", user.Id);
                throw new RepositoryException(nameof(UserRepository), user.Id,
                    $"User with ID {user.Id} not found for update");
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Successfully updated user with ID: {UserId}", user.Id);
            return user;
        }
        catch (RepositoryException)
        {
            // TODO Review this - Here I am just rethrowing the exception, not sure if this is appropriate    
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict while updating user with ID: {UserId}", user.Id);
            throw new DatabaseException(UpdateOperation, TableName, "The user was modified by another process",
                ex.ToString());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database constraint violation while updating user with ID: {UserId}", user.Id);

            if (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                throw new DatabaseException(UpdateOperation, TableName,
                    "A user with this email already exists in the database",
                    ex.ToString());

            throw new DatabaseException(UpdateOperation, TableName, "Failed to update user in database", ex.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating user with ID: {UserId}", user.Id);
            throw new DatabaseException(UpdateOperation, TableName, "Unexpected error occurred while updating user",
                ex.ToString());
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
            throw new DatabaseException(DeleteOperation, TableName, $"Failed to delete user with ID: {id}",
                ex.ToString());
        }
    }
}