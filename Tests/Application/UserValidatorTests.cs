using Aristotle.Application;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Exceptions;
using Aristotle.UnitTests.Builders;
using Xunit;

namespace Aristotle.UnitTests.Application;

public class UserValidatorTests
{
    [Fact]
    public async Task ValidateUserAsync_ValidUser_ShouldNotThrowException()
    {
        // Arrange
        var validUser = new UserBuilder().WithId().WithEmailAddress().WithAdultAge().WithName().Build();

        // Act & Assert
        await UserValidator.ValidateUserAsync(validUser);
        // If we reach here, no exception was thrown
        Assert.True(true);
    }

    [Fact]
    public async Task ValidateUserAsync_ValidUserWithoutDateOfBirth_ShouldNotThrowException()
    {
        // Arrange
        var validUser = new UserBuilder().WithId().WithEmailAddress().WithName().Build();

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
        var user = new UserBuilder().WithId().WithEmailAddress().WithAdultAge().Build();
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
        var user = new UserBuilder().WithId().WithEmailAddress().WithAdultAge().Build();

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
        var user = new UserBuilder().WithId().WithEmailAddress().WithAdultAge().WithName().Build();
        user.Name = new string('A', 131); // 131 characters

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
        var user = new UserBuilder().WithId().WithEmailAddress().WithAdultAge().Build();
        user.Name = new string('A', 100); // Changed from 130 to 100 characters

        // Act & Assert
        await UserValidator.ValidateUserAsync(user);
        Assert.True(true);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ValidateUserAsync_EmptyOrWhitespaceEmail_ShouldThrowDomainValidationException(string? email)
    {
        // Arrange
        var user = new UserBuilder().WithId().WithAdultAge().WithName().Build();
        user.Email = email!;

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
        var user = new UserBuilder().WithId().WithAdultAge().WithName().Build();
        user.Email = invalidEmail;

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
        var user = new UserBuilder().WithName().WithId().WithAdultAge().Build();
        user.Email = validEmail;

        // Act & Assert
        await UserValidator.ValidateUserAsync(user);
        Assert.True(true);
    }

    [Fact]
    public async Task ValidateUserAsync_FutureDateOfBirth_ShouldThrowDomainValidationException()
    {
        // Arrange
        var user = new UserBuilder()
            .WithId()
            .WithName()
            .WithEmailAddress()
            .WithDateOfBirth(DateTime.Now.AddDays(1))
            .Build();

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
        var user = new UserBuilder()
            .WithId()
            .WithName()
            .WithEmailAddress()
            .WithDateOfBirth(DateTime.Now.AddYears(-131))
            .Build();

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
        var user = new UserBuilder()
            .WithId()
            .WithDateOfBirth(DateTime.Now.AddDays(1))
            .Build();
        user.Name = "";
        user.Email = "invalid-email";

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
        var user = new UserBuilder()
            .WithId()
            .WithName()
            .WithEmailAddress()
            .WithDateOfBirth(DateTime.Now.Date)
            .Build();

        // Act & Assert
        await UserValidator.ValidateUserAsync(user);
        Assert.True(true);
    }
}