using System.Net.Mail;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Interfaces;
using Aristotle.Domain.Exceptions;
using Aristotle.Application.Exceptions;

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

            _logger.LogInformation("Getting user with ID: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);

            // Switch seems overkill here, but it can be useful for future extensibility
            // or if you want to log different messages based on the result.
            // In this case, we only have two possible outcomes: success or not found.
            // Also I wanted to avoid The logging message template should not vary between calls warning.
            switch (user)
            {
                case null:
                    _logger.LogInformation("User with ID {UserId} not found", id);
                    break;
                default:
                    _logger.LogInformation("Successfully retrieved user with ID: {UserId}", id);
                    break;
            }

            return user;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogError(ex, "Error occurred while getting user with ID: {UserId}", id);

            throw new ServiceOperationException(nameof(UserService), nameof(GetUserByIdAsync),
                "An error occurred while retrieving the user.", ex);
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

            _logger.LogDebug("Getting user by email lookup initiated");
            var user = await _userRepository.GetByEmailAsync(email);
            
            return user;
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            _logger.LogError(ex, "Error occurred while getting user by email lookup");

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
            if (user == null)
            {
                _logger.LogWarning("Attempted to create null user");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            await ValidateUserAsync(user);
            
            await ValidateUniqueEmailAsync(user.Email);

            _logger.LogDebug("Creating user initiated");
            var createdUser = await _userRepository.AddAsync(user);

            return createdUser;
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
    public async Task<User> UpdateUserAsync(User user)
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
                _logger.LogWarning("Attempted to update non-existent user with ID: {UserId}", user.Id);
                throw new UserNotFoundException(user.Id);
            }

            await ValidateUserAsync(user);

            if (existingUser.Email != user.Email) await ValidateUniqueEmailAsync(user.Email);

            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);
            var updatedUser = await _userRepository.UpdateAsync(user);
            
            return updatedUser;
        }
        catch (Exception ex) when (ex is not (ArgumentNullException or UserNotFoundException
                                       or DomainValidationException or DuplicateUserEmailException))
        {
            _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", user?.Id ?? Guid.Empty);

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
    /// Validates user data according to domain rules.
    /// This method checks that required fields are present and properly formatted.
    /// </summary>
    /// <param name="user">The user to validate.</param>
    /// <exception cref="DomainValidationException">Thrown when validation fails.</exception>
    private static async Task ValidateUserAsync(User user)
    {
        var validationErrors = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(user.Name))
            validationErrors.Add(nameof(User.Name), ["Name is required and cannot be empty."]);
        else if (user.Name.Length > 100)
            validationErrors.Add(nameof(User.Name), ["Name cannot exceed 100 characters."]);

        if (string.IsNullOrWhiteSpace(user.Email))
            validationErrors.Add(nameof(User.Email), ["Email is required and cannot be empty."]);
        else if (!IsValidEmail(user.Email)) validationErrors.Add(nameof(User.Email), ["Email format is invalid."]);

        if (user.DateOfBirth.HasValue)
        {
            if (user.DateOfBirth.Value > DateTime.Now)
                validationErrors.Add(nameof(User.DateOfBirth),
                    ["Date of birth cannot be in the future."]);
            else if (user.DateOfBirth.Value < DateTime.Now.AddYears(-150))
                validationErrors.Add(nameof(User.DateOfBirth),
                    ["Date of birth cannot be more than 150 years ago."]);
        }
        // I think throwing this specific exception is not useful, as the validation errors dictionary contains all the
        // information needed to understand what went wrong. (in theory I mean)
        //But as this is a learning exercise, I will keep it this way (also awareness of PII data and GDPR is important
        // when throwing exceptions with user data)
        if (validationErrors.Count != 0) throw new DomainValidationException(validationErrors, nameof(User));

        await Task.CompletedTask;
    }

    /// <summary>
    /// Validates that the provided email is unique in the system.
    /// </summary>
    /// <param name="email">The email to check for uniqueness.</param>
    /// <exception cref="DuplicateUserEmailException">Thrown when a user with the same email already exists.</exception>
    private async Task ValidateUniqueEmailAsync(string email)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            throw new DuplicateUserEmailException(email);
        }

        _logger.LogDebug("Email validation passed for unique email check");
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
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}