using Aristotle.Controllers;
using Aristotle.Domain.Entities;
using Aristotle.Application.Service;
using Aristotle.Application.DTOs;
using Aristotle.UnitTests.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AutoMapper;

namespace Aristotle.UnitTests.Application.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _serviceMock;
    private readonly Mock<ILogger<UserController>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _serviceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _mapperMock = new Mock<IMapper>();
        _controller = new UserController(_serviceMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    // ------------------------ Constructor Tests ------------------------//
    // Validates if the constructor throws ArgumentNullException when any dependency is null
    // related to dependencie injection and service setup.
    [Fact]
    public void Constructor_NullService_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UserController(null!, _loggerMock.Object, _mapperMock.Object));
    }

    [Fact]
    public void Constructor_NullLogger_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UserController(_serviceMock.Object, null!, _mapperMock.Object));
    }

    [Fact]
    public void Constructor_NullMapper_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UserController(_serviceMock.Object, _loggerMock.Object, null!));
    }


    // ------------------------ Action Method Tests ------------------------//
    // Test each action method for expected behavior, including success and failure scenarios.
    [Fact]
    public async Task GetUserByEmail_ReturnsOk()
    {
        // Arrange
        var userBuilder = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress();
        var user = userBuilder.Build();
        var userResponse = userBuilder.BuildResponseDto();

        _serviceMock.Setup(s => s.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userResponse);

        // Act
        var result = await _controller.GetUserByEmail(user.Email);

        // Assert
        _serviceMock.Verify(s => s.GetUserByEmailAsync(user.Email), Times.Once);
        _mapperMock.Verify(m => m.Map<UserResponseDto>(user), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(userResponse, ok.Value);
    }

    [Fact]
    public async Task GetUserById_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userBuilder = new UserBuilder().WithAdultAge().WithId(id).WithName().WithEmailAddress();
        var user = userBuilder.Build();
        var userResponse = userBuilder.BuildResponseDto();

        _serviceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userResponse);

        // Act
        var result = await _controller.GetUserById(id);
        var ok = Assert.IsType<OkObjectResult>(result);

        //Assert 
        _serviceMock.Verify(s => s.GetUserByIdAsync(id), Times.Once);
        _mapperMock.Verify(m => m.Map<UserResponseDto>(user), Times.Once);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(userResponse, ok.Value);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated()
    {
        // Arrange
        var userCreateDto = new UserBuilder().WithAdultAge().WithName().WithEmailAddress().BuildCreateDto();
        var user = new UserBuilder()
            .WithId()
            .WithEmailAddress(userCreateDto.Email)
            .WithName(userCreateDto.Name)
            .WithDateOfBirth(userCreateDto.DateOfBirth)
            .Build();
        var userResponse = new UserBuilder()
            .WithId(user.Id)
            .WithEmailAddress(user.Email)
            .WithName(user.Name)
            .WithDateOfBirth(user.DateOfBirth)
            .BuildResponseDto();

        _mapperMock.Setup(m => m.Map<User>(userCreateDto)).Returns(user);
        _serviceMock.Setup(s => s.CreateUserAsync(user)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userResponse);

        // Act
        var result = await _controller.CreateUser(userCreateDto);
        var created = Assert.IsType<CreatedAtActionResult>(result);

        // Assert
        _mapperMock.Verify(m => m.Map<User>(userCreateDto), Times.Once);
        _serviceMock.Verify(s => s.CreateUserAsync(user), Times.Once);
        _mapperMock.Verify(m => m.Map<UserResponseDto>(user), Times.Once);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(userResponse, created.Value);
        Assert.Equal(nameof(UserController.GetUserById), created.ActionName);
        Assert.Equal(user.Id, (Guid)created.RouteValues!["id"]!);
    }

    [Fact]
    public async Task CreateUser_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");
        var userCreateDto = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().BuildCreateDto();

        // Act
        var result = await _controller.CreateUser(userCreateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        var modelState = Assert.IsType<SerializableError>(badRequestResult.Value, false);
        Assert.True(modelState.ContainsKey("Name"));
        Assert.Contains("Required", ((string[])modelState["Name"])[0]);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userUpdateDto = new UserBuilder().WithAdultAge().WithName().WithEmailAddress().BuildUserUpdateDto();
        var user = new UserBuilder()
            .WithId(id)
            .WithEmailAddress(userUpdateDto.Email)
            .WithName(userUpdateDto.Name)
            .WithDateOfBirth(userUpdateDto.DateOfBirth)
            .Build();
        var userResponse = new UserBuilder()
            .WithId(user.Id)
            .WithEmailAddress(user.Email)
            .WithName(user.Name)
            .WithDateOfBirth(user.DateOfBirth)
            .BuildResponseDto();

        _mapperMock.Setup(m => m.Map<User>(userUpdateDto)).Returns(user);
        _serviceMock.Setup(s => s.UpdateUserAsync(user)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userResponse);

        // Act
        var result = await _controller.UpdateUser(id, userUpdateDto);

        // Assert
        _mapperMock.Verify(m => m.Map<User>(userUpdateDto), Times.Once);
        _serviceMock.Verify(s => s.UpdateUserAsync(user), Times.Once);
        _mapperMock.Verify(m => m.Map<UserResponseDto>(user), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userResponse, ok.Value);
    }

    [Fact]
    public async Task UpdateUser_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("e", "err");
        var userUpdateDto = new UserBuilder().WithAdultAge().WithId().WithName().WithInvalidEmail()
            .BuildUserUpdateDto();

        // Act
        var result = await _controller.UpdateUser(Guid.NewGuid(), userUpdateDto);

        // Assert
        Assert.Equal(400, (result as BadRequestObjectResult)!.StatusCode);
        var modelState = Assert.IsType<SerializableError>(((BadRequestObjectResult)result).Value!, false);
        Assert.True(modelState.ContainsKey("e"));
        Assert.Contains("err", ((string[])modelState["e"])[0]);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var userUpdateDto = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress()
            .BuildUserUpdateDto();
        const string errorMessage = "The ID in the URL must match the ID in the user data.";
        _controller.ModelState.AddModelError("Id", errorMessage);

        // Act
        var result = await _controller.UpdateUser(Guid.NewGuid(), userUpdateDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        var modelState = Assert.IsType<SerializableError>(badRequestResult.Value, false);
        Assert.True(modelState.ContainsKey("Id"));
        Assert.Contains(errorMessage, ((string[])modelState["Id"])[0]);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(id);

        // Assert
        _serviceMock.Verify(s => s.DeleteUserAsync(id), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteUser_NotFound_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(id);

        // Assert
        _serviceMock.Verify(s => s.DeleteUserAsync(id), Times.Once);
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), nf.Value?.ToString());
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOk()
    {
        // Arrange
        var users = new List<User>
        {
            new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build(),
            new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build(),
            new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().Build()
        };
        var userResponse = users.Select(u => new UserBuilder()
                .WithId(u.Id)
                .WithName(u.Name)
                .WithEmailAddress(u.Email)
                .WithDateOfBirth(u.DateOfBirth)
                .BuildResponseDto())
            .ToList();
        _serviceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(userResponse);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        _serviceMock.Verify(s => s.GetAllUsersAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<UserResponseDto>>(users), Times.Once);
        Assert.IsType<OkObjectResult>(result);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userResponse, ok.Value);
    }
}