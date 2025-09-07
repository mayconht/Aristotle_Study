using Aristotle.Application;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Exceptions;
using Xunit;

namespace Aristotle.UnitTests.Application;

public class UserValidatorTests
{
    [Fact]
    public async Task ValidateUserAsync_ValidUser_ShouldNotThrowException()
    {
        // Arrange
        var validUser = new User("test@example.com", "John Doe")
        {
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act & Assert
        await UserValidator.ValidateUserAsync(validUser);
        // If we reach here, no exception was thrown
        Assert.True(true);
    }

    [Fact]
    public async Task ValidateUserAsync_ValidUserWithoutDateOfBirth_ShouldNotThrowException()
    {
        // Arrange
        var validUser = new User("test@example.com", "John Doe")
        {
            DateOfBirth = null
        };

        // Act & Assert
        await UserValidator.ValidateUserAsync(validUser);
        Assert.True(true);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task ValidateUserAsync_EmptyOrWhitespaceName_ShouldThrowDomainValidationException(string name)
    {
        // Arrange
        var user = new User("test@example.com", "Valid Name");
        user.Name = name; // Override with invalid name

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("Name", exception.ValidationErrors.Keys);
        Assert.Contains("Name is required and cannot be empty.", exception.ValidationErrors["Name"]);
    }

    [Fact]
    public async Task ValidateUserAsync_NullName_ShouldThrowDomainValidationException()
    {
        // Arrange
        var user = new User("test@example.com", "Valid Name");
        user.Name = null!; // Override with null name

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("Name", exception.ValidationErrors.Keys);
        Assert.Contains("Name is required and cannot be empty.", exception.ValidationErrors["Name"]);
    }

    [Fact]
    public async Task ValidateUserAsync_NameTooLong_ShouldThrowDomainValidationException()
    {
        // Arrange
        var longName = new string('A', 101); // 101 characters
        var user = new User("test@example.com", longName);

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("Name", exception.ValidationErrors.Keys);
        Assert.Contains("Name cannot exceed 100 characters.", exception.ValidationErrors["Name"]);
    }

    [Fact]
    public async Task ValidateUserAsync_MaxLengthName_ShouldNotThrowException()
    {
        // Arrange
        var maxLengthName = new string('A', 100); // Exactly 100 characters
        var user = new User("test@example.com", maxLengthName);

        // Act & Assert
        await UserValidator.ValidateUserAsync(user);
        Assert.True(true);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task ValidateUserAsync_EmptyOrWhitespaceEmail_ShouldThrowDomainValidationException(string email)
    {
        // Arrange
        var user = new User("valid@example.com", "John Doe");
        user.Email = email; // Override with invalid email

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("Email", exception.ValidationErrors.Keys);
        Assert.Contains("Email is required and cannot be empty.", exception.ValidationErrors["Email"]);
    }

    [Fact]
    public async Task ValidateUserAsync_NullEmail_ShouldThrowDomainValidationException()
    {
        // Arrange
        var user = new User("valid@example.com", "John Doe");
        user.Email = null!; // Override with null email

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("Email", exception.ValidationErrors.Keys);
        Assert.Contains("Email is required and cannot be empty.", exception.ValidationErrors["Email"]);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test@.com")]
    public async Task ValidateUserAsync_InvalidEmailFormat_ShouldThrowDomainValidationException(string invalidEmail)
    {
        // Arrange
        var user = new User(invalidEmail, "John Doe");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("Email", exception.ValidationErrors.Keys);
        Assert.Contains("Email format is invalid.", exception.ValidationErrors["Email"]);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.com")]
    [InlineData("test.email@example-domain.com")]
    [InlineData("user123@test123.org")]
    public async Task ValidateUserAsync_ValidEmailFormats_ShouldNotThrowException(string validEmail)
    {
        // Arrange
        var user = new User(validEmail, "John Doe");

        // Act & Assert
        await UserValidator.ValidateUserAsync(user);
        Assert.True(true);
    }

    [Fact]
    public async Task ValidateUserAsync_FutureDateOfBirth_ShouldThrowDomainValidationException()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        var user = new User("test@example.com", "John Doe")
        {
            DateOfBirth = futureDate
        };

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("DateOfBirth", exception.ValidationErrors.Keys);
        Assert.Contains("Date of birth cannot be in the future.", exception.ValidationErrors["DateOfBirth"]);
    }

    [Fact]
    public async Task ValidateUserAsync_DateOfBirthTooOld_ShouldThrowDomainValidationException()
    {
        // Arrange
        var tooOldDate = DateTime.Now.AddYears(-131);
        var user = new User("test@example.com", "John Doe")
        {
            DateOfBirth = tooOldDate
        };

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        Assert.Contains("DateOfBirth", exception.ValidationErrors.Keys);
        Assert.Contains("Date of birth cannot be more than 130 years ago.", exception.ValidationErrors["DateOfBirth"]);
    }

    [Fact]
    public async Task ValidateUserAsync_MultipleValidationErrors_ShouldThrowExceptionWithAllErrors()
    {
        // Arrange
        var user = new User("invalid-email", ""); // Invalid email and empty name
        user.DateOfBirth = DateTime.Now.AddDays(1); // Future date

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<DomainValidationException>(() => UserValidator.ValidateUserAsync(user));

        // Should have errors for Name, Email, and DateOfBirth
        Assert.Equal(3, exception.ValidationErrors.Count);
        Assert.Contains("Name", exception.ValidationErrors.Keys);
        Assert.Contains("Email", exception.ValidationErrors.Keys);
        Assert.Contains("DateOfBirth", exception.ValidationErrors.Keys);
    }

    [Fact]
    public async Task ValidateUserAsync_EdgeCaseDateOfBirth_ShouldNotThrowException()
    {
        // Arrange
        var todayDate = DateTime.Now.Date;
        var user = new User("test@example.com", "John Doe")
        {
            DateOfBirth = todayDate
        };

        // Act & Assert
        await UserValidator.ValidateUserAsync(user);
        Assert.True(true);
    }
}