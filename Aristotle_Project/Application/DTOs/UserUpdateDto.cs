using System.ComponentModel.DataAnnotations;

namespace Aristotle.Application.DTOs;

/// <summary>
/// Data Transfer Object for updating an existing user.
/// Used for API requests to decouple the domain model from the API layer.
/// </summary>
public class UserUpdateDto
{
  /// <summary>
    /// Constructor for UserUpdateDto.
    /// </summary>
    public UserUpdateDto()
    {
    }

    //TODO: Probably on future we will not have those as required, as we will use PATCH verb for updates
    // https://stackoverflow.com/questions/28459418/use-of-put-vs-patch-methods-in-rest-api-real-life-scenarios
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
    public DateTime? DateOfBirth { get; init; }
}