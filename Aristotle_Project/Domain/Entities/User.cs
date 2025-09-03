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
    public Guid Id { get; set; }

    /// <summary>
    /// User Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// User Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// User Date of Birth.
    /// This property is optional and can be null.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
}