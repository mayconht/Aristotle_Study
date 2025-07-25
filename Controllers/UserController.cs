using Aristotle.Application.Service;
using Aristotle.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Aristotle.Controllers;

/// <summary>
/// UserController.
/// After I added the Swagger lib it complained about the lack of XML comments,
/// so I added some basic comments to the methods.
/// This controller provides endpoints for creating, retrieving, updating, and deleting users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    /// <summary>
    /// Constructor for UserController.
    /// </summary>
    /// <param name="userService"></param>
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets a user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        // CreatedAtAction returns a 201 Created response with a location header pointing to the newly created resource.
        // It also includes the created user in the response body.
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null) return NotFound();

        var updatedUser = await _userService.UpdateUserAsync(user);
        return Ok(updatedUser);
    }

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (await _userService.DeleteUserAsync(id)) return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        if (!users.Any()) return NotFound();

        return Ok(users);
    }
}