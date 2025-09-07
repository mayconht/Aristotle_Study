using Aristotle.Application.Exceptions;
using Aristotle.Application.Service;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Exceptions;
using Aristotle.Domain.Interfaces;
using Aristotle.UnitTests.Builders;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Aristotle.UnitTests.Application.Service;

public class UserServiceTests
{
    private static readonly Guid UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly Mock<ILogger<UserService>> _loggerMock;

    private readonly User _user = new("test@example.com", "Test User")
    {
        Id = UserId,
        DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync(_user);

        // Act
        var result = await _userService.GetUserByIdAsync(UserId);

        // Assert
        Assert.NotNull(result);
        // I truly like to use verify lib to handle object verification
        // but this one is small enough to use Assert directly
        Assert.Equal(_user.Id, result.Id);
        Assert.Equal(_user.Name, result.Name);
        Assert.Equal(_user.Email, result.Email);
        Assert.Equal(_user.DateOfBirth, result.DateOfBirth);

        //This one is quite annoying to configure and understand what is going on, but 
        // it is important to assert the exception handling and logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Getting user with ID: {UserId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowArgumentException_ForEmptyId()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByIdAsync(Guid.Empty));

        // Verify
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Attempted to get user with empty ID")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowUserNotFoundException_WhenNotExists()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.GetUserByIdAsync(UserId));
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldWrapException_OnRepoError()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(UserId)).ThrowsAsync(new InvalidOperationException());

        // Act & Assert
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.GetUserByIdAsync(UserId));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(_user.Email)).ReturnsAsync(_user);

        // Act
        var result = await _userService.GetUserByEmailAsync(_user.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_user.Id, result.Id);
        Assert.Equal(_user.Name, result.Name);
        Assert.Equal(_user.Email, result.Email);
        Assert.Equal(_user.DateOfBirth, result.DateOfBirth);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetUserByEmailAsync_ShouldThrowArgumentNullException_OnBlank(string? email)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.GetUserByEmailAsync(email!));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldThrowArgumentException_OnInvalidFormat()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByEmailAsync("bad"));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldThrowUserEmailNotFoundException_WhenNotExists()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(_user.Email)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UserEmailNotFoundException>(() => _userService.GetUserByEmailAsync(_user.Email));
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldWrapException_OnRepoError()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(_user.Email)).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.GetUserByEmailAsync(_user.Email));
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnTrue_WhenDeleted()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.DeleteAsync(UserId)).ReturnsAsync(true);

        // Act
        var res = await _userService.DeleteUserAsync(UserId);

        // Assert
        Assert.True(res);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.DeleteAsync(UserId)).ReturnsAsync(false);

        // Act
        var res = await _userService.DeleteUserAsync(UserId);

        // Assert
        Assert.False(res);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowArgumentException_ForEmptyId()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteUserAsync(Guid.Empty));
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldWrapException_OnRepoError()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.DeleteAsync(UserId)).ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.DeleteUserAsync(UserId));
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowServiceOperationException_WhenNull()
    {
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.CreateUserAsync(null!));
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowArgumentException_OnInvalidUser()
    {
        // Arrange
        var inv = new UserBuilder().WithName(" ").WithEmailAddress("bad")
            .WithDateOfBirth(DateTime.Now.AddYears(200)).Build();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(inv));
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowDuplicateUserEmailException_WhenExists()
    {
        // Arrange
        var nu = new UserBuilder().WithEmailAddress().WithId().WithName().Build();
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(nu.Email)).ReturnsAsync(nu);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUserEmailException>(() => _userService.CreateUserAsync(nu));
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnUser_WhenValid()
    {
        // Arrange
        var nu = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();
        
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(nu.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.AddAsync(nu)).ReturnsAsync(nu);

        // Act
        var res = await _userService.CreateUserAsync(nu);

        // Assert
        Assert.Equal(nu, res);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowArgumentNullException_WhenNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateUserAsync(null!));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowUserNotFoundException_WhenNotExists()
    {
        // Arrange
        var t = new UserBuilder().Build();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(t.Id)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _userService.UpdateUserAsync(t));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowDomainValidationException_OnInvalidUser()
    {
        // Arrange
        var existingUser = new UserBuilder().Build();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(existingUser.Id)).ReturnsAsync(existingUser);
        var invalidUser = new UserBuilder().WithName(null!).WithEmailAddress(" ").Build();
        // Use the same Id to make validation runs instead of UserNotFoundException
        invalidUser.Id = existingUser.Id;

        // Act & Assert
        await Assert.ThrowsAsync<DomainValidationException>(() => _userService.UpdateUserAsync(invalidUser));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowDuplicateUserEmailException_WhenChanging()
    {
        // Arrange
        var ex = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();
        var other = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(ex.Id)).ReturnsAsync(ex);
        
        var up = new User(ex.Email, ex.Name)
        {
            Id = ex.Id,
            Email = other.Email
        };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(other.Email)).ReturnsAsync(other);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUserEmailException>(() => _userService.UpdateUserAsync(up));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnUpdatedUser_WhenValid()
    {
        // Arrange
        var ex = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();
        ex.Id = UserId;
        var up = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();
        up.Id = UserId;
        _userRepositoryMock.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync(ex);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(up.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.UpdateAsync(up)).ReturnsAsync(up);

        // Act
        var res = await _userService.UpdateUserAsync(up);

        // Assert
        Assert.Equal(up, res);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnList_WhenExists()
    {
        // Arrange
        var users = new List<User> { _user };
        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = (await _userService.GetAllUsersAsync()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(_user, result[0]);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Getting all users")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Successfully retrieved {users.Count} users")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldWrapException_OnRepoError()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.GetAllUsersAsync());
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error occurred while getting all users")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldWrapException_OnRepoAddError()
    {
        // Arrange
       var nu = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(nu.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.AddAsync(nu)).ThrowsAsync(new Exception("Add error"));

        // Act & Assert
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.CreateUserAsync(nu));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error occurred while creating user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldWrapException_OnRepoUpdateError()
    {
        // Arrange
        var existing =  new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();

        existing.Id = UserId;
        var up = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build();
        up.Id = UserId;
        _userRepositoryMock.Setup(r => r.GetByIdAsync(UserId)).ReturnsAsync(existing);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(up.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.UpdateAsync(up)).ThrowsAsync(new Exception("Update error"));

        // Act & Assert
        await Assert.ThrowsAsync<ServiceOperationException>(() => _userService.UpdateUserAsync(up));
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error occurred while updating user")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}