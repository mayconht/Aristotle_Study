using Aristotle.Domain.Entities;
using Aristotle.Domain.Exceptions;

namespace Aristotle.Application;

/// <summary>
/// Validator class for User entity business rules.
/// </summary>
public static class UserValidator
{
    /// <summary>
    /// Validates user data according to domain rules.
    /// </summary>
    /// <param name="user">The user to validate.</param>
    /// <exception cref="DomainValidationException">Thrown when validation fails.</exception>
    public static async Task ValidateUserAsync(User user)
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
            else if (user.DateOfBirth.Value < DateTime.Now.AddYears(-130))
                validationErrors.Add(nameof(User.DateOfBirth),
                    ["Date of birth cannot be more than 130 years ago."]);
        }

        if (validationErrors.Count != 0) throw new DomainValidationException(validationErrors, nameof(User));

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
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}