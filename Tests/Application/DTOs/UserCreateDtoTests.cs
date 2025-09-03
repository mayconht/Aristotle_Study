using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Aristotle.Application.DTOs;
using Xunit;

namespace Aristotle.UnitTests.Application.DTOs;

public class UserCreateDtoTests
{
    [Fact]
    public void Constructor_WithParameters_SetsProperties()
    {
        var name = "Test User";
        var email = "test@example.com";
        var dob = new DateTime(2000, 1, 1);
        var dto = new UserCreateDto
        {
            Name = name,
            Email = email,
            DateOfBirth = dob
        };
        Assert.Equal(name, dto.Name);
        Assert.Equal(email, dto.Email);
        Assert.Equal(dob, dto.DateOfBirth);
    }

    [Fact]
    public void ParameterlessConstructor_AllowsPropertySet()
    {
        var dto = new UserCreateDto
        {
            Name = "Test User",
            Email = "test@example.com",
            DateOfBirth = new DateTime(1999, 12, 31)
        };
        Assert.Equal("Test User", dto.Name);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(new DateTime(1999, 12, 31), dto.DateOfBirth);
    }

    [Fact]
    public void CanSerializeAndDeserialize()
    {
        var dto = new UserCreateDto
            {
                Name = "A",
                Email = "a@b.com",
                DateOfBirth = new DateTime(1995, 5, 5)
            }
            ;
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<UserCreateDto>(json);
        Assert.Equal(dto.Name, deserialized!.Name);
        Assert.Equal(dto.Email, deserialized.Email);
        Assert.Equal(dto.DateOfBirth, deserialized.DateOfBirth);
    }

    [Fact]
    public void DataAnnotations_ValidObject_PassesValidation()
    {
        var dto = new UserCreateDto
        {
            Name = "Valid Name",
            Email = "valid@email.com",
            DateOfBirth = null
        };
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
        var dto = new UserCreateDto
        {
            Name = name!,
            Email = email!,
            DateOfBirth = null
        };
        var ctx = new ValidationContext(dto);
        Assert.Throws<ValidationException>(() => Validator.ValidateObject(dto, ctx, true));
    }
}