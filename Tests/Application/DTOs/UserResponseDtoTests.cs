using System;
using System.Text.Json;
using Aristotle.Application.DTOs;
using Xunit;

namespace Aristotle.UnitTests.Application.DTOs;

public class UserResponseDtoTests
{
    [Fact]
    public void Constructor_WithParameters_SetsProperties()
    {
        var id = Guid.NewGuid();
        var name = "Test User";
        var email = "test@example.com";
        var dob = new DateTime(2000, 1, 1, 0, 0, 1, DateTimeKind.Utc);
        var dto = new UserResponseDto()
        {
            Id = id,
            Name = name,
            Email = email,
            DateOfBirth = dob
        };
        Assert.Equal(id, dto.Id);
        Assert.Equal(name, dto.Name);
        Assert.Equal(email, dto.Email);
        Assert.Equal(dob, dto.DateOfBirth);
    }

    [Fact]
    public void ParameterlessConstructor_AllowsPropertySet()
    {
        var id = Guid.NewGuid();
        var dto = new UserResponseDto
        {
            Id = id,
            Name = "Test User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(1999, 12, 31, 0, 0, 1, DateTimeKind.Utc)
        };
        Assert.Equal(id, dto.Id);
        Assert.Equal("Test User", dto.Name);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(new DateTime(1999, 12, 31, 0, 0, 1, DateTimeKind.Utc), dto.DateOfBirth);
    }

    [Fact]
    public void CanSerializeAndDeserialize()
    {
        var dto = new UserResponseDto()
        {
            Name = "A",
            Email = "a@b.com",
            DateOfBirth = new DateTime(1995, 5, 5, 0, 0, 1, DateTimeKind.Utc)
        };
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<UserResponseDto>(json);
        Assert.Equal(dto.Id, deserialized!.Id);
        Assert.Equal(dto.Name, deserialized.Name);
        Assert.Equal(dto.Email, deserialized.Email);
        Assert.Equal(dto.DateOfBirth, deserialized.DateOfBirth);
    }
}