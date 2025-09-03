using Aristotle.Domain.Exceptions;
using Xunit;

namespace Aristotle.UnitTests.Domain.Exceptions;

public class UserDomainExceptionTests
{
    [Fact]
    public void DuplicateUserEmailException_ShouldSetMessageAndErrorCode()
    {
        // Arrange
        var email = "duplicate@example.com";

        // Act
        var ex = new DuplicateUserEmailException(email);

        // Assert
        Assert.Equal($"A user with email '{email}' already exists.", ex.Message);
        Assert.Equal(nameof(DuplicateUserEmailException), ex.ErrorCode);
    }

    [Fact]
    public void UserNotFoundException_ShouldSetMessageAndErrorCodeAndProperties()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var ex = new UserNotFoundException(id);

        // Assert
        var expectedMessage = $"The User with identifier '{id}' was not found.";
        Assert.Equal(expectedMessage, ex.Message);
        Assert.Equal(nameof(UserNotFoundException), ex.ErrorCode);
        Assert.Equal("User", ex.EntityType);
        Assert.Equal(id, ex.EntityId);
    }

    [Fact]
    public void UserEmailNotFoundException_ShouldSetMessageAndErrorCodeAndProperties()
    {
        // Arrange
        var email = "notfound@example.com";

        // Act
        var ex = new UserEmailNotFoundException(email);

        // Assert
        var expectedMessage = $"The User with identifier '{email}' was not found.";
        Assert.Equal(expectedMessage, ex.Message);
        Assert.Equal(nameof(UserEmailNotFoundException), ex.ErrorCode);
        Assert.Equal("User", ex.EntityType);
        Assert.Equal(email, ex.EntityId);
    }
}