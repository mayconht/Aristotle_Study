using Aristotle.Application.DTOs;
using Aristotle.Domain.Entities;
using Bogus;

namespace UserService.UnitTests.Builders;

/// <summary>
/// Builder class for creating User objects in tests following the Fluent Interface pattern.
/// Uses Bogus library to generate "realistic" fake data.
/// Example usage: new UserBuilder().WithEmailAddress("test@test.com").WithName("Test User").Build()
/// Example usage with Faker: new UserBuilder().WithValidEmail().WithAdultAge().Build()
/// </summary>
public class UserBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private string? _name;
    private string? _email;
    private DateTime? _dateOfBirth;

    /// <summary>
    /// Sets the user ID.
    /// </summary>
    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the user ID to a new GUID.
    /// </summary>
    public UserBuilder WithId()
    {
        _id = Guid.NewGuid();
        return this;
    }

    /// <summary>
    /// Sets the use name.
    /// </summary>
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the use name using Faker to generate a realistic name.
    /// </summary>
    public UserBuilder WithName()
    {
        _name = _faker.Name.FullName();
        return this;
    }

    /// <summary>
    /// Sets the user email address.
    /// </summary>
    public UserBuilder WithEmailAddress(string email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Sets the user email address using Faker to generate a realistic email.
    /// </summary>
    public UserBuilder WithEmailAddress()
    {
        _email = _faker.Internet.Email();
        return this;
    }

    /// <summary>
    /// Sets the user date of birth.
    /// </summary>
    public UserBuilder WithDateOfBirth(DateTime? dateOfBirth)
    {
        _dateOfBirth = dateOfBirth;
        return this;
    }

    /// <summary>
    /// Sets an invalid email format for testing validation.
    /// </summary>
    public UserBuilder WithInvalidEmail()
    {
        _email = "invalid-email";
        return this;
    }

    /// <summary>
    /// Generates a date of birth for an adult (18-65 years old).
    /// </summary>
    public UserBuilder WithAdultAge()
    {
        _dateOfBirth = _faker.Date.Between(DateTime.Today.AddYears(-65), DateTime.Today.AddYears(-18));
        return this;
    }

    /// <summary>
    /// Generates a date of birth for a minor (< 18 years old).
    /// </summary>
    public UserBuilder WithMinorAge()
    {
        _dateOfBirth = _faker.Date.Between(DateTime.Today.AddYears(-17), DateTime.Today.AddDays(-1));
        return this;
    }

    /// <summary>
    /// Generates fake data for a specific locale.
    /// </summary>
    /// <param name="locale">The locale for fake data (e.g., "pt_BR", "en", "es")</param>
    //TODO: This is not being used currently, but will be cool if I manage to use it in some tests and add realistic
    //names in different languages or information related to locale.
    public UserBuilder WithFakeDataForLocale(string locale)
    {
        var localeFaker = new Faker(locale);
        _name = localeFaker.Name.FullName();
        _email = localeFaker.Internet.Email();
        _dateOfBirth = localeFaker.Date.Between(DateTime.Today.AddYears(-65), DateTime.Today.AddYears(-18));
        return this;
    }

    /// <summary>
    /// Builds the User object with the configured properties.
    /// </summary>
    public User Build()
    {
        return new User()
        {
            Id = _id,
            Name = _name!,
            Email = _email!,
            DateOfBirth = _dateOfBirth
        };
    }

    /// <Summary>
    /// Builds a UserCreateDto with the configured properties.
    /// </Summary>
    public UserCreateDto BuildCreateDto()
    {
        return new UserCreateDto
        {
            Name = _name!,
            Email = _email!,
            DateOfBirth = _dateOfBirth
        };
    }

    /// <Summary>
    /// Builds a UserResponseDto with the configured properties.
    /// </Summary>
    public UserResponseDto BuildResponseDto()
    {
        return new UserResponseDto
        {
            Id = _id,
            Name = _name!,
            Email = _email!,
            DateOfBirth = _dateOfBirth
        };
    }

    /// <Summary>
    /// Builds a UserUpdateDto with the configured properties.
    /// </Summary>
    public UserUpdateDto BuildUserUpdateDto()
    {
        return new UserUpdateDto
        {
            Name = _name!,
            Email = _email!,
            DateOfBirth = _dateOfBirth
        };
    }
}