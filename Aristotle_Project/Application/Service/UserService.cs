using System.Net.Mail;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Interfaces;
using Aristotle.Domain.Exceptions;
using Aristotle.Application.Exceptions;
using Aristotle.Application;

namespace Aristotle.Application.Service;

// Here we define the UserService class, which is responsible for handling user-related operations.
// Usually if a business logic is needed, it should be placed here.
// If you need to add more complex operations, you can create a separate class for that purpose.

/// <summary>
/// Service class that handles user-related business operations.
/// Provides methods for CRUD operations on users with proper exception handling
/// and business logic validation following hexagonal architecture principles.
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of the UserService class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access operations.</param>
    /// <param name="logger">Logger for service operations and error tracking.</param>
    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The user if found, null otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided ID is empty.</exception>
    /// <exception cref="ServiceOperationException">Thrown when the operation fails due to infrastructure issues.</exception>
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get user with empty ID");
                throw new ArgumentException("User ID cannot be empty.", nameof(id));
            }

            _logger.LogDebug("Getting user with ID: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new UserNotFoundException(id);

            return user;
        }
        catch (Exception ex)
        {
            if (ex is ArgumentException or UserNotFoundException)
                throw;

            _logger.LogError(ex, "Error occurred while getting user by ID: {UserId}", id);
            throw new ServiceOperationException(nameof(UserService), nameof(GetUserByIdAsync),
                "An error occurred while retrieving the user by ID.", ex);
        }
    }

    /// <summary>
    /// Gets user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user if found, null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the email parameter is null or empty.</exception>
    /// <exception cref="ServiceOperationException">Thrown when the operation fails due to infrastructure issues.</exception>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Attempted to get user with null or empty email");
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");
            }

            if (!IsValidEmail(email))
            {
                _logger.LogDebug("Invalid email format provided: {Email}", email);
                _logger.LogWarning("Invalid email format provided");
                throw new ArgumentException("Email format is invalid.", nameof(email));
            }

            _logger.LogDebug("Getting user by email");
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new UserEmailNotFoundException(email);

            return user;
        }
        catch (Exception ex)
        {
            if (ex is ArgumentNullException or ArgumentException or UserEmailNotFoundException)
                throw;

            _logger.LogError(ex, "Error occurred while getting user by email: {Email}", email);
            throw new ServiceOperationException(nameof(UserService), nameof(GetUserByEmailAsync),
                "An error occurred while retrieving the user by email.", ex);
        }
    }

    // Sometimes the amount of asynchronous operations reminds me of callback hell,
    // why async to await to async to await? anyway...
    /// <summary>
    /// Gets all users from the system.
    /// </summary>
    /// <returns>A collection of all users in the system.</returns>
    /// <exception cref="ServiceOperationException">Thrown when the operation fails due to infrastructure issues.</exception>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all users");

            var users = await _userRepository.GetAllAsync();
            var allUsersAsync = users.ToList();

            _logger.LogDebug("Successfully retrieved {UserCount} users", allUsersAsync.Count);

            return allUsersAsync;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");

            throw new ServiceOperationException(nameof(UserService), nameof(GetAllUsersAsync),
                "An error occurred while retrieving all users.", ex);
        }
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>The created user with generated ID.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the user parameter is null.</exception>
    /// <exception cref="DomainValidationException">Thrown when user data validation fails.</exception>
    /// <exception cref="DuplicateUserEmailException">Thrown when a user with the same email already exists.</exception>
    /// <exception cref="ServiceOperationException">Thrown when the operation fails due to infrastructure issues.</exception>
    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email format is invalid provided for new user.");

            await ValidateUniqueEmailAsync(user.Email);

            _logger.LogDebug("Creating user initiated");
            var createdUser = await _userRepository.AddAsync(user);

            return createdUser;
        }
        catch (ArgumentException ex)
        {
            _logger.LogDebug(ex, "Invalid argument provided while creating user");
            throw new ArgumentException("Invalid argument provided while creating user", ex);
        }
        catch (Exception ex) when (ex is not (ArgumentNullException or DomainValidationException
                                       or DuplicateUserEmailException))
        {
            _logger.LogError(ex, "Error occurred while creating user");

            throw new ServiceOperationException(nameof(UserService), nameof(CreateUserAsync),
                "An error occurred while creating the user.", ex);
        }
    }

    /// <summary>
    /// Updates an existing user in the system.
    /// </summary>
    /// <param name="user">The user entity with updated information.</param>
    /// <returns>The updated user entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the user parameter is null.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user to update is not found.</exception>
    /// <exception cref="DomainValidationException">Thrown when user data validation fails.</exception>
    /// <exception cref="DuplicateUserEmailException">Thrown when updating to an email that already exists.</exception>
    /// <exception cref="ServiceOperationException">Thrown when the operation fails due to infrastructure issues.</exception>
    public async Task<User> UpdateUserAsync(User? user)
    {
        try
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to update null user");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var existingUser = await _userRepository.GetByIdAsync(user.Id);

            if (existingUser == null)
            {
                _logger.LogDebug("Attempted to update non-existent user with ID: {UserId}", user.Id);
                _logger.LogWarning("Attempted to update non-existent user");
                throw new UserNotFoundException(user.Id);
            }

            await UserValidator.ValidateUserAsync(user);

            if (existingUser.Email != user.Email) await ValidateUniqueEmailAsync(user.Email);

            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);
            var updatedUser = await _userRepository.UpdateAsync(user);

            return updatedUser;
        }
        catch (Exception ex) when (ex is not (ArgumentNullException or UserNotFoundException
                                       or DomainValidationException or DuplicateUserEmailException))
        {
            _logger.LogDebug(ex, "Error occurred while updating user with ID: {UserId}", user?.Id ?? Guid.Empty);
            _logger.LogError(ex, "Error occurred while updating user");

            throw new ServiceOperationException(nameof(UserService), nameof(UpdateUserAsync),
                "An error occurred while updating the user.", ex);
        }
    }

    /// <summary>
    /// Deletes a user from the system by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>True if the user was deleted successfully, false if the user was not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided ID is empty.</exception>
    /// <exception cref="ServiceOperationException">Thrown when the operation fails due to infrastructure issues.</exception>
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete user with empty ID");
                throw new ArgumentException("User ID cannot be empty.", nameof(id));
            }

            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var result = await _userRepository.DeleteAsync(id);

            switch (result)
            {
                case true:
                    _logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
                    break;
                default:
                    _logger.LogInformation("User with ID {UserId} not found for deletion", id);
                    break;
            }

            return result;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);

            throw new ServiceOperationException(nameof(UserService), nameof(DeleteUserAsync),
                "An error occurred while deleting the user.", ex);
        }
    }

    //-------------------------------------------------Business Logic-------------------------------------------------//

    //This should be moved to a separate validation service or utility class
    // In this case as it is a crud service, I will keep it here for simplicity and that is why they are private methods.


    /// <summary>
    /// Validates that the provided email is unique in the system.
    /// </summary>
    /// <param name="email">The email to check for uniqueness.</param>
    /// <exception cref="DuplicateUserEmailException">Thrown when a user with the same email already exists.</exception>
    private async Task ValidateUniqueEmailAsync(string email)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null) throw new DuplicateUserEmailException(email);

        _logger.LogDebug("Email validation passed for unique email check");

        await Task.CompletedTask;
    }

    /// <summary>
    /// Validates email format using a simple regex pattern.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>True if the email format is valid, false otherwise.</returns>
    private static bool IsValidEmail(string email)
    {
        try
        {
            // Just learned about MailAddress class, it is a simple way to validate email format
            //It is not perfect, but it is better than a regex
            // More info: https://learn.microsoft.com/en-us/dotnet/api/system.net.mail
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}