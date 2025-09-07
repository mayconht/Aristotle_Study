using Aristotle.Domain.Entities;
using Aristotle.UnitTests.Builders;
using Xunit;

namespace Aristotle.UnitTests.Domain.Entities;

public class UserTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidEmailAndName_ShouldCreateUser()
    {
        // Arrange
        const string email = "test@example.com";
        const string name = "Test User";

        // Act
        var user = new UserBuilder()
            .WithEmailAddress(email)
            .WithName(name)
            .Build();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.Equal(Guid.Empty, user.Id);
        Assert.Null(user.DateOfBirth);
    }

    [Fact]
    public void Constructor_WithEmptyEmail_ShouldCreateUser()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithEmailAddress("")
            .WithName("Test User")
            .Build();

        // Assert
        Assert.NotNull(user);
        Assert.Equal("", user.Email);
    }

    [Fact]
    public void Constructor_WithNullEmail_ShouldCreateUser()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithEmailAddress(null!)
            .WithName("Test User")
            .Build();

        // Assert
        Assert.NotNull(user);
        Assert.Null(user.Email);
    }

    [Fact]
    public void Constructor_WithNullName_ShouldCreateUser()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithEmailAddress("test@example.com")
            .WithName(null!)
            .Build();

        // Assert
        Assert.NotNull(user);
        Assert.Null(user.Name);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Id_Property_CanBeSetAndRetrieved()
    {
        // Arrange
        var user = new UserBuilder().WithName().WithEmailAddress().Build();
        var newId = Guid.NewGuid();

        // Act
        user.Id = newId;

        // Assert
        Assert.Equal(newId, user.Id);
    }

    [Fact]
    public void Name_Property_CanBeSetAndRetrieved()
    {
        // Arrange
        var user = new UserBuilder().WithEmailAddress().Build();
        const string newName = "Updated Name";

        // Act
        user.Name = newName;

        // Assert
        Assert.Equal(newName, user.Name);
    }

    [Fact]
    public void Email_Property_CanBeSetAndRetrieved()
    {
        // Arrange
        var user = new UserBuilder().WithName().Build();
        const string newEmail = "updated@example.com";

        // Act
        user.Email = newEmail;

        // Assert
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void DateOfBirth_Property_CanBeSetAndRetrieved()
    {
        // Arrange
        var user = new UserBuilder().WithName().WithEmailAddress().Build();
        var dateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        user.DateOfBirth = dateOfBirth;

        // Assert
        Assert.Equal(dateOfBirth, user.DateOfBirth);
    }

    [Fact]
    public void DateOfBirth_Property_CanBeSetToNull()
    {
        // Arrange
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress()
            .WithDateOfBirth(DateTime.Today)
            .Build();

        // Act
        user.DateOfBirth = null;

        // Assert
        Assert.Null(user.DateOfBirth);
    }

    #endregion

    #region Builder Integration Tests

    [Fact]
    public void UserBuilder_WithEmailAddress_ShouldSetEmail()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress("builder@example.com")
            .Build();

        // Assert
        Assert.Equal("builder@example.com", user.Email);
    }

    [Fact]
    public void UserBuilder_WithValidEmail_ShouldSetValidEmail()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress()
            .Build();

        // Assert
        Assert.NotNull(user.Email);
        Assert.NotEmpty(user.Email);
        Assert.Contains("@", user.Email);
        Assert.Contains(".", user.Email);
    }

    [Fact]
    public void UserBuilder_WithInvalidEmail_ShouldSetInvalidEmail()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithName()
            .WithInvalidEmail()
            .Build();

        // Assert
        Assert.Equal("invalid-email", user.Email);
    }

    [Fact]
    public void UserBuilder_WithAdultAge_ShouldSetDateOfBirth()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress()
            .WithAdultAge()
            .Build();

        // Assert
        Assert.NotNull(user.DateOfBirth);
        var age = DateTime.Today.Year - user.DateOfBirth.Value.Year;
        Assert.True(age >= 18);
    }

    [Fact]
    public void UserBuilder_WithMinorAge_ShouldSetDateOfBirth()
    {
        // Arrange & Act
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress()
            .WithMinorAge()
            .Build();

        // Assert
        Assert.NotNull(user.DateOfBirth);
        var age = DateTime.Today.Year - user.DateOfBirth.Value.Year;
        if (user.DateOfBirth.Value.DayOfYear > DateTime.Today.DayOfYear) age--;
        Assert.True(age < 18);
    }

    [Fact]
    public void UserBuilder_CompleteChain_ShouldCreateUserWithAllProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string name = "Complete User";
        const string email = "complete@example.com";
        var dateOfBirth = new DateTime(1985, 6, 15, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var user = new UserBuilder()
            .WithId(id)
            .WithName(name)
            .WithEmailAddress(email)
            .WithDateOfBirth(dateOfBirth)
            .Build();

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(dateOfBirth, user.DateOfBirth);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void User_WithVeryLongName_ShouldAccept()
    {
        // Arrange
        var longName = new string('a', 1000);

        // Act
        var user = new UserBuilder()
            .WithEmailAddress()
            .WithName(longName)
            .Build();

        // Assert
        Assert.Equal(longName, user.Name);
    }

    [Fact]
    public void User_WithVeryLongEmail_ShouldAccept()
    {
        // Arrange
        var longEmail = new string('a', 200) + "@example.com";

        // Act
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress(longEmail)
            .Build();

        // Assert
        Assert.Equal(longEmail, user.Email);
    }

    [Fact]
    public void User_WithFutureDateOfBirth_ShouldAccept()
    {
        // Arrange
        var futureDate = DateTime.Today.AddYears(10);

        // Act
        var user = new UserBuilder()
            .WithName()
            .WithEmailAddress()
            .WithDateOfBirth(futureDate)
            .Build();

        // Assert
        Assert.Equal(futureDate, user.DateOfBirth);
    }

    #endregion
}