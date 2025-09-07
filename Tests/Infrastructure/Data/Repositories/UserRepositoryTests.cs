using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Infrastructure;
using Aristotle.Infrastructure.Exceptions;
using Aristotle.UnitTests.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


namespace Aristotle.UnitTests.Infrastructure.Data.Repositories;

public class UserRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options =
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private readonly Mock<ILogger<UserRepository>> _loggerMock = new();

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _loggerMock.Object));
        Assert.Equal("context", exception.ParamName);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        var exception =
            Assert.Throws<ArgumentNullException>(() => new UserRepository(new ApplicationDbContext(_options), null!));
        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void Constructor_DoesNotThrow_WhenParametersAreValid()
    {
        var exception =
            Record.Exception(() => new UserRepository(new ApplicationDbContext(_options), _loggerMock.Object));
        Assert.Null(exception);
    }


    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        var user = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.GetByIdAsync(user.Id);
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        await using var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        var result = await repo.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        var u1 = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        var u2 = new UserBuilder().WithId().WithEmailAddress().WithName().Build();

        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.AddRange(u1, u2);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var list = (await repo.GetAllAsync()).ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, x => x.Id == u1.Id);
            Assert.Contains(list, x => x.Id == u2.Id);
        }
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        var u = new UserBuilder().WithId().WithEmailAddress("test@x.com").WithName().Build();

        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.Add(u);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.GetByEmailAsync(u.Email);
            Assert.NotNull(result);
            Assert.Equal(u.Id, result.Id);
            Assert.Equal(u.Name, result.Name);
            Assert.Equal(u.Email, result.Email);
        }
    }

    [Fact]
    public async Task AddAsync_AddsUser_WhenValid()
    {
        var u = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.AddAsync(u);
            Assert.Equal(u.Id, result.Id);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var saved = await context.Users.FindAsync([u.Id], TestContext.Current.CancellationToken);
            Assert.NotNull(saved);
            Assert.Equal(u.Id, saved.Id);
        }
    }

    [Fact]
    public async Task AddAsync_ThrowsArgumentNullException_WhenNull()
    {
        await using var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null!));
    }

    [Fact]
    public async Task AddAsync_ThrowsDatabaseException_OnContextDisposed()
    {
        var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        await context.DisposeAsync();
        var u = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await Assert.ThrowsAsync<DatabaseException>(() => repo.AddAsync(u));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenNull()
    {
        await using var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => repo.UpdateAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsRepositoryException_WhenNotExists()
    {
        await using var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        var u = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await Assert.ThrowsAsync<RepositoryException>(() => repo.UpdateAsync(u));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenExists()
    {
        var originalUser = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.Add(originalUser);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var updatedUser = new UserBuilder().WithId(originalUser.Id).WithEmailAddress("updated@example.com").WithName("Updated Name").Build();
        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.UpdateAsync(updatedUser);
            Assert.NotNull(result);
            Assert.Equal(updatedUser.Id, result.Id);
            Assert.Equal(updatedUser.Name, result.Name);
            Assert.Equal(updatedUser.Email, result.Email);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var savedUser = await context.Users.FindAsync([originalUser.Id], TestContext.Current.CancellationToken);
            Assert.NotNull(savedUser);
            Assert.Equal(updatedUser.Name, savedUser.Name);
            Assert.Equal(updatedUser.Email, savedUser.Email);
        }
    }

    [Fact]
    public async Task UpdateAsync_ThrowsDatabaseException_OnContextDisposed()
    {
        var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        var u = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await context.DisposeAsync();
        await Assert.ThrowsAsync<DatabaseException>(() => repo.UpdateAsync(u));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
    {
        await using var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        var result = await repo.DeleteAsync(Guid.NewGuid());
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenExists()
    {
        var user = new UserBuilder().WithId().WithEmailAddress().WithName().Build();
        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.DeleteAsync(user.Id);
            Assert.True(result);
            var deleted = await context.Users.FindAsync([user.Id], TestContext.Current.CancellationToken);
            Assert.Null(deleted);
        }
    }

    [Fact]
    public async Task DeleteAsync_ThrowsDatabaseException_OnContextDisposed()
    {
        var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        var id = Guid.NewGuid();
        await context.DisposeAsync();
        await Assert.ThrowsAsync<DatabaseException>(() => repo.DeleteAsync(id));
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsDatabaseException_OnContextDisposed()
    {
        var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        await context.DisposeAsync();
        await Assert.ThrowsAsync<DatabaseException>(() => repo.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ThrowsDatabaseException_OnContextDisposed()
    {
        var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        await context.DisposeAsync();
        await Assert.ThrowsAsync<DatabaseException>(() => repo.GetAllAsync());
    }

    [Fact]
    public async Task GetByEmailAsync_ThrowsDatabaseException_WhenEmailInvalid()
    {
        await using var context = new ApplicationDbContext(_options);
        var repo = new UserRepository(context, _loggerMock.Object);
        await Assert.ThrowsAsync<DatabaseException>(() => repo.GetByEmailAsync("invalid-email"));
    }
}