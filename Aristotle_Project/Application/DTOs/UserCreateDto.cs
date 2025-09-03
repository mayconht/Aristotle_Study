using System.ComponentModel.DataAnnotations;

namespace Aristotle.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new user.
/// Used for API requests to decouple the domain model from the API layer.
/// </summary>
public class UserCreateDto
{

    /// <summary>
    /// Constructor for UserCreateDto.
    /// </summary>
    public UserCreateDto()
    {
    }

    /// <summary>
    /// User name.
    /// </summary>
    [Required]
    [StringLength(130)]
    public required string Name { get; init; }

    /// <summary>
    /// User email.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public required string Email { get; init; }

    /// <summary>
    /// User date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}