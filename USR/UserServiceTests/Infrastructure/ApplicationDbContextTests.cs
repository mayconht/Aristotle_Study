using Aristotle.Domain.Entities;
using Aristotle.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UserService.UnitTests.Infrastructure;

public class ApplicationDbContextTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Users);
    }

    [Fact]
    public void Users_Property_ShouldBeConfigured()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestUsersProperty")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Users);
    }

    #endregion

    #region Model Configuration Tests

    [Fact]
    public void OnModelCreating_ShouldConfigureUserEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestModelCreating")
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act
        var model = context.Model;
        var userEntity = model.FindEntityType(typeof(User));

        // Assert
        Assert.NotNull(userEntity);
        Assert.Equal("User", userEntity.GetTableName());
    }

    [Fact]
    public void OnModelCreating_UserEntity_ShouldHaveCorrectKeyConfiguration()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestKeyConfig")
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act
        var model = context.Model;
        var userEntity = model.FindEntityType(typeof(User));
        var primaryKey = userEntity?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties[0].Name);
    }

    [Fact]
    public void OnModelCreating_UserEntity_ShouldHaveCorrectPropertyConfigurations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestPropertyConfig")
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act
        var model = context.Model;
        var userEntity = model.FindEntityType(typeof(User));

        var nameProperty = userEntity?.FindProperty("Name");
        var emailProperty = userEntity?.FindProperty("Email");
        var dateOfBirthProperty = userEntity?.FindProperty("DateOfBirth");

        // Assert
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
        Assert.Equal(130, nameProperty.GetMaxLength());

        Assert.NotNull(emailProperty);
        Assert.False(emailProperty.IsNullable);
        Assert.Equal(200, emailProperty.GetMaxLength());

        Assert.NotNull(dateOfBirthProperty);
        Assert.True(dateOfBirthProperty.IsNullable);
    }

    [Fact]
    public void OnModelCreating_UserEntity_ShouldHaveUniqueEmailIndex()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestEmailIndex")
            .Options;

        using var context = new ApplicationDbContext(options);

        // Act
        var model = context.Model;
        var userEntity = model.FindEntityType(typeof(User));
        var emailIndex = userEntity?.GetIndexes().FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));

        // Assert
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
        Assert.Single(emailIndex.Properties);
        Assert.Equal("Email", emailIndex.Properties[0].Name);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_ShouldDisposeContextProperly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDispose")
            .Options;

        ApplicationDbContext context;

        // Act
        using (context = new ApplicationDbContext(options))
        {
            Assert.NotNull(context);
        }

        Assert.Throws<ObjectDisposedException>(() => context.Users.Count());
    }

    #endregion
}