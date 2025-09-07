using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Aristotle.Application.DTOs;
using Aristotle.UnitTests.Builders;
using Xunit;

namespace Aristotle.UnitTests.Application.DTOs;

// Sometimes testing a DTO feels redundant, but it's important to ensure
// that serialization, data annotations, and property behaviors work as expected.
// sometimes DTOs have logic or constraints that need verification too.
public class UserCreateDtoTests
{
    [Fact]
    public void Constructor_WithParameters_SetsProperties()
    {
        // Arrange
        var name = "Test User";
        var email = "test@example.com";
        var dob = new DateTime(2000, 1, 1, 0, 0, 1, DateTimeKind.Utc);
        
        // Act
        var dto = new UserCreateDto
        {
            Name = name,
            Email = email,
            DateOfBirth = dob
        };
        
        // Assert
        Assert.Equal(name, dto.Name);
        Assert.Equal(email, dto.Email);
        Assert.Equal(dob, dto.DateOfBirth);
    }

    [Fact]
    public void ParameterlessConstructor_AllowsPropertySet()
    {
        // Arrange & Act
        var dto = new UserCreateDto
        {
            Name = "Test User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(1999, 12, 31, 0, 0, 1, DateTimeKind.Utc)
        };
        
        // Assert
        Assert.Equal("Test User", dto.Name);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(new DateTime(1999, 12, 31, 0, 0, 1, DateTimeKind.Utc), dto.DateOfBirth);
    }

    [Fact]
    public void CanSerializeAndDeserialize()
    {
        // Arrange
        var dto = new UserCreateDto
            {
                Name = "A",
                Email = "a@b.com",
                DateOfBirth = new DateTime(1995, 5, 5, 0, 0, 1, DateTimeKind.Utc)
            };
        
        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<UserCreateDto>(json);
        
        // Assert
        Assert.Equal(dto.Name, deserialized!.Name);
        Assert.Equal(dto.Email, deserialized.Email);
        Assert.Equal(dto.DateOfBirth, deserialized.DateOfBirth);
    }

    [Fact]
    public void DataAnnotations_ValidObject_PassesValidation()
    {
        // Arrange
        var dto = new UserBuilder().WithAdultAge().WithId().WithName().WithEmailAddress().BuildCreateDto();
        
        // Act & Assert
        var ctx = new ValidationContext(dto);
        Validator.ValidateObject(dto, ctx, true);
    }

    [Theory]
    [InlineData(null, "a@b.com")]
    [InlineData("", "a@b.com")]
    [InlineData("Test", null)]
    [InlineData("Test", "")]
    [InlineData("Test", "not-an-email")]
    public void DataAnnotations_InvalidObject_Throws(string? name, string? email)
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Name = name!,
            Email = email!,
            DateOfBirth = null
        };
        
        // Act & Assert
        var ctx = new ValidationContext(dto);
        Assert.Throws<ValidationException>(() => Validator.ValidateObject(dto, ctx, true));
    }
}