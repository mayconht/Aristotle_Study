using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aristotle.Domain.Entities;
// References: https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations

// Feels weird to have it in plural (maybe a language thing),
// but if you think the table is for users and the class is for the user object, it makes sense

/// <summary>
/// A user entity representing a person in the system.
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// User constructor.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="name"></param>
    public User(string email, string name)
    {
        Email = email;
        Name = name;
    }

    /// <summary>
    /// User Generated Id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// User Name.
    /// </summary>
    [Required]
    public string Name { get; init; }

    /// <summary>
    /// User Email.
    /// </summary>
    [Required]
    public string Email { get; init; }

    /// <summary>
    /// User Date of Birth.
    /// This property is optional and can be null.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}