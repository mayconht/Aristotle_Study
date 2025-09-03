using Aristotle.Controllers;
using Aristotle.Domain.Entities;
using Aristotle.Application.Service;
using Aristotle.Application.DTOs;
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

    [Fact]
    public async Task GetUserByEmail_ReturnsOk()
    {
        var email = "test@example.com";
        var user = new User(email, "Name") { Id = Guid.NewGuid() };
        var userDto = new UserResponseDto { Id = user.Id, Name = user.Name, Email = user.Email };
        _serviceMock.Setup(s => s.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

        var result = await _controller.GetUserByEmail(email);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(userDto, ok.Value);
    }

    [Fact]
    public async Task GetUserById_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var user = new User("e@x.com", "Name") { Id = id };
        var userDto = new UserResponseDto { Id = user.Id, Name = user.Name, Email = user.Email };
        _serviceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

        var result = await _controller.GetUserById(id);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(userDto, ok.Value);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated()
    {
        var userCreateDto = new UserCreateDto { Name = "Name", Email = "abc@x.com" };
        var user = new User(userCreateDto.Email, userCreateDto.Name) { Id = Guid.NewGuid() };
        var userDto = new UserResponseDto { Id = user.Id, Name = user.Name, Email = user.Email };
        _mapperMock.Setup(m => m.Map<User>(userCreateDto)).Returns(user);
        _serviceMock.Setup(s => s.CreateUserAsync(user)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

        var result = await _controller.CreateUser(userCreateDto);
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(userDto, created.Value);
        Assert.Equal(nameof(UserController.GetUserById), created.ActionName);
        Assert.Equal(user.Id, (Guid)created.RouteValues!["id"]!);
    }

    [Fact]
    public async Task CreateUser_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var userCreateDto = new UserCreateDto { Name = "", Email = "a@x.com" };
        var result = await _controller.CreateUser(userCreateDto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var userUpdateDto = new UserUpdateDto { Name = "Name", Email = "a@x.com" };
        var user = new User(userUpdateDto.Email, userUpdateDto.Name) { Id = id };
        var userDto = new UserResponseDto { Id = user.Id, Name = user.Name, Email = user.Email };
        _mapperMock.Setup(m => m.Map<User>(userUpdateDto)).Returns(user);
        _serviceMock.Setup(s => s.UpdateUserAsync(user)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

        var result = await _controller.UpdateUser(id, userUpdateDto);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDto, ok.Value);
    }

    [Fact]
    public async Task UpdateUser_InvalidModel_ReturnsBadRequest()
    {
        _controller.ModelState.AddModelError("e", "err");
        var userUpdateDto = new UserUpdateDto { Name = "Name", Email = "e@x.com" };
        var result = await _controller.UpdateUser(Guid.NewGuid(), userUpdateDto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(true);

        var result = await _controller.DeleteUser(id);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteUser_NotFound_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteUserAsync(id)).ReturnsAsync(false);

        var result = await _controller.DeleteUser(id);
        var nf = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), nf.Value?.ToString());
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOk()
    {
        var users = new List<User> { new("e@x.com", "Name") };
        var userDtos = users.Select(u => new UserResponseDto { Id = u.Id, Name = u.Name, Email = u.Email }).ToList();
        _serviceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(userDtos);

        var result = await _controller.GetAllUsers();
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(userDtos, ok.Value);
    }
}