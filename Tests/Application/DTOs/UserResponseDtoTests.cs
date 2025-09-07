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
        // Arrange
        var id = Guid.NewGuid();
        var name = "Test User";
        var email = "test@example.com";
        var dob = new DateTime(2000, 1, 1, 0, 0, 1, DateTimeKind.Utc);

        // Act
        var dto = new UserResponseDto()
        {
            Id = id,
            Name = name,
            Email = email,
            DateOfBirth = dob
        };

        // Assert
        Assert.Equal(id, dto.Id);
        Assert.Equal(name, dto.Name);
        Assert.Equal(email, dto.Email);
        Assert.Equal(dob, dto.DateOfBirth);
    }

    [Fact]
    public void CanSerializeAndDeserialize()
    {
        // Arrange
        var dto = new UserResponseDto()
        {
            Name = "A",
            Email = "a@b.com",
            DateOfBirth = new DateTime(1995, 5, 5, 0, 0, 1, DateTimeKind.Utc)
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<UserResponseDto>(json);

        // Assert
        Assert.Equal(dto.Id, deserialized!.Id);
        Assert.Equal(dto.Name, deserialized.Name);
        Assert.Equal(dto.Email, deserialized.Email);
        Assert.Equal(dto.DateOfBirth, deserialized.DateOfBirth);
    }
}
