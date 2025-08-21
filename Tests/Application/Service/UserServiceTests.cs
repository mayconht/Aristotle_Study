using Aristotle.Application.Service;
using Aristotle.Domain.Entities;
using Aristotle.Domain.Interfaces;
using Aristotle.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aristotle.UnitTests.Application.Service;

/// <summary>
/// Tests for UserService functionality
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _userService;
    private static readonly Guid UserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private readonly User _user = new("test@example.com", "Test User")
    {
        Id = UserId,
        DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    };

    /// <summary>
    /// Setup for all UserService tests
    /// </summary>
    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Test to verify that GetUserByIdAsync returns a user when the user exists
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(UserId)).ReturnsAsync(_user);

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
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Getting user with ID: {UserId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);


        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Successfully retrieved user with ID: {UserId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Test to verify that GetUserByIdAsync returns null when the user does not exist
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(UserId)).ReturnsAsync((User?)null);
        // Act
        var result = await _userService.GetUserByIdAsync(UserId);
        // Assert
        Assert.Null(result);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains($"Getting user with ID: {UserId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("User with ID ") &&
                    v.ToString()!.Contains(UserId.ToString()) &&
                    v.ToString()!.Contains("not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(UserId), Times.Once);
    }

    /// <summary>
    /// Test to verify that GetUserByIdAsync throws ServiceOperationException when the repository throws an exception
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowServiceOperationException_WhenRepositoryThrowsException()
    {
        // Arrange
        var expectedException = new Exception("Database connection failed");
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(UserId)).ThrowsAsync(expectedException);

        // Act
        var exception =
            await Assert.ThrowsAsync<ServiceOperationException>(async () =>
                await _userService.GetUserByIdAsync(UserId));

        // Assert
        // In Java is super tricky to assert exceptions, I like the way C# handles it
        Assert.NotNull(exception);
        Assert.Equal("UserService", exception.Service);
        Assert.Equal("GetUserByIdAsync", exception.Operation);
        Assert.Equal("An error occurred while retrieving the user.", exception.Message);
        Assert.Same(expectedException, exception.InnerException);

        // I still feel that this is too verbose
        // TODO: Validate if there is a way to simplify this
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains($"Getting user with ID: {UserId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains($"Error occurred while getting user with ID: {UserId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(UserId), Times.Once);
    }
}