using Aristotle.Application;
using Aristotle.Application.DTOs;
using Aristotle.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aristotle.UnitTests.Application;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        var serviceProvider = services.BuildServiceProvider();
        _mapper = serviceProvider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void User_To_UserDto_Maps_All_Properties()
    {
        var user = new User("test@example.com", "Test User")
        {
            Id = Guid.NewGuid(),
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 1, DateTimeKind.Utc)
        };

        var dto = _mapper.Map<UserResponseDto>(user);

        Assert.Equal(user.Id, dto.Id);
        Assert.Equal(user.Name, dto.Name);
        Assert.Equal(user.Email, dto.Email);
        Assert.Equal(user.DateOfBirth, dto.DateOfBirth);
    }

    [Fact]
    public void UserCreateDto_To_User_Maps_All_Properties()
    {
        var dto = new UserCreateDto
        {
            Name = "Test User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 1, DateTimeKind.Utc)
        };

        var user = _mapper.Map<User>(dto);

        Assert.Equal(dto.Name, user.Name);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.DateOfBirth, user.DateOfBirth);
    }

    [Fact]
    public void UserUpdateDto_To_User_Maps_All_Properties()
    {
        var dto = new UserUpdateDto
        {
            Name = "Updated User",
            Email = "updated@example.com",
            DateOfBirth = new DateTime(1995, 5, 5, 0, 0, 1, DateTimeKind.Utc)
        };

        var user = _mapper.Map<User>(dto);

        Assert.Equal(dto.Name, user.Name);
        Assert.Equal(dto.Email, user.Email);
        Assert.Equal(dto.DateOfBirth, user.DateOfBirth);
    }
}