using Aristotle.Application.Service;
using Aristotle.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Aristotle.Controllers;

/// <summary>
/// UserController provides REST API endpoints for user management operations.
/// All exceptions are handled by the global exception handling middleware.
/// This controller focuses on HTTP-specific concerns like routing, status codes, and response formatting.
/// After I added the Swagger lib it complained about the lack of XML comments,
/// so I added some basic comments to the methods.
/// This controller provides endpoints for creating, retrieving, updating, and deleting users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Initializes a new instance of the UserController class.
    /// </summary>
    /// <param name="userService">The user service for business operations.</param>
    /// <param name="logger">Logger for controller operations.</param>
    public UserController(UserService userService, ILogger<UserController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>The user if found, or a 404 Not Found response if the user doesn't exist.</returns>
    /// <response code="200">User retrieved successfully.</response>
    /// <response code="400">Invalid email address provided.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        _logger.LogInformation("Received request to get user by email: {Email}", email);    
        

        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            _logger.LogInformation("User with email {Email} not found", email);
            return NotFound($"User with email {email} was not found.");
        }
        _logger.LogInformation("Successfully retrieved user with email: {Email}", email);
        return Ok(user);
    }
    
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>The user if found, or a 404 Not Found response if the user doesn't exist.</returns>
    /// <response code="200">User retrieved successfully.</response>
    /// <response code="400">Invalid user ID provided.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        _logger.LogInformation("Received request to get user by ID: {UserId}", id);

        // The service will handle validation and throw appropriate exceptions
        // which will be caught by the global exception handler so we dont need to worry about throwing exceptions here.
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            _logger.LogInformation("User with ID {UserId} not found", id);
            return NotFound($"User with ID {id} was not found.");
        }

        _logger.LogInformation("Successfully retrieved user with ID: {UserId}", id);
        return Ok(user);
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">The user data to create.</param>
    /// <returns>The created user with a 201 Created response including location header.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid user data provided or validation failed.</response>
    /// <response code="409">User with the same email already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        _logger.LogInformation("Received request to create user with email: {Email}", user.Email);

        // Model state validation is performed automatically by ASP.NET Core
        // If the model state is invalid, it will return a 400 Bad Request response. 
        // This includes checks for required fields, data types, and validation attributes, quite useful for ensuring the integrity of the data.
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state validation failed for create user request");
            return BadRequest(ModelState);
        }

        var createdUser = await _userService.CreateUserAsync(user);

        _logger.LogInformation("Successfully created user with ID: {UserId}", createdUser.Id);

        // CreatedAtAction returns a 201 Created response with a location header pointing to the newly created resource.
        // It also includes the created user in the response body.
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    /// <summary>
    /// Updates an existing user in the system.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="user">The updated user data.</param>
    /// <returns>The updated user data.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="400">Invalid user data provided or ID mismatch.</response>
    /// <response code="404">User not found.</response>
    /// <response code="409">Email conflict with another user.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
    {
        _logger.LogInformation("Received request to update user with ID: {UserId}", id);


        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state validation failed for update user request");
            return BadRequest(ModelState);
        }

        if (user.Id != id)
        {
            _logger.LogWarning("ID mismatch: URL ID {UrlId} vs User ID {UserId}", id, user.Id);
            return BadRequest("The ID in the URL must match the ID in the user data.");
        }

        var updatedUser = await _userService.UpdateUserAsync(user);

        _logger.LogInformation("Successfully updated user with ID: {UserId}", updatedUser.Id);
        return Ok(updatedUser);
    }

    /// <summary>
    /// Deletes a user from the system by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>204 No Content if deleted successfully, 404 Not Found if user doesn't exist.</returns>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="400">Invalid user ID provided.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        _logger.LogInformation("Received request to delete user with ID: {UserId}", id);

        var deleted = await _userService.DeleteUserAsync(id);

        if (!deleted)
        {
            _logger.LogInformation("User with ID {UserId} not found for deletion", id);
            return NotFound($"User with ID {id} was not found.");
        }

        _logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves all users from the system.
    /// </summary>
    /// <returns>A collection of all users in the system.</returns>
    /// <response code="200">Users retrieved successfully (includes empty collection).</response>
    /// <response code="500">Internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("Received request to get all users");

        var users = await _userService.GetAllUsersAsync();

        _logger.LogInformation("Successfully retrieved {UserCount} users", users.Count());

        // Return OK even if no users exist - an empty collection is still a valid response
        return Ok(users);
    }
}