namespace Aristotle.Application.DTOs;

//TODO: This is a temporary DTO, as I plan to use CreateUserRequest and UpdateUserRequest in the future
// to separate the concerns and make the project readable, I hope this is not a Clean Architecture sin.
/// <summary>
/// Data Transfer Object for User entity.
/// Used for API responses to decouple the domain model from the API layer.
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// Empty constructor for UserDto.
    /// </summary>
    public UserResponseDto()
    {
    }

    /// <summary>
    /// User unique identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// User email.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// User date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; init; }
}