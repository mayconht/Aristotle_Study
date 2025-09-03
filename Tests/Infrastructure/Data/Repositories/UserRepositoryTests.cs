using Aristotle.Infrastructure.Data.Repositories;
using Aristotle.Infrastructure;
using Aristotle.Domain.Entities;
using Aristotle.Infrastructure.Exceptions;
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
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        var user = new User("e@x.com", "Name") { Id = Guid.NewGuid() };
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
        var u1 = new User("a@x.com", "A") { Id = Guid.NewGuid() };
        var u2 = new User("b@x.com", "B") { Id = Guid.NewGuid() };
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
        var email = "test@x.com";
        var u = new User(email, "Name") { Id = Guid.NewGuid() };
        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.Add(u);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.GetByEmailAsync(email);
            Assert.NotNull(result);
            Assert.Equal(u.Id, result.Id);
            Assert.Equal(u.Name, result.Name);
            Assert.Equal(u.Email, result.Email);
        }
    }

    [Fact]
    public async Task AddAsync_AddsUser_WhenValid()
    {
        var u = new User("add@x.com", "Name") { Id = Guid.NewGuid() };
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
        var u = new User("x@x.com", "Name") { Id = Guid.NewGuid() };
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
        var u = new User("e@x.com", "Name") { Id = Guid.NewGuid() };
        await Assert.ThrowsAsync<RepositoryException>(() => repo.UpdateAsync(u));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser_WhenExists()
    {
        var id = Guid.NewGuid();
        var originalUser = new User("o@x.com", "Old") { Id = id };
        await using (var context = new ApplicationDbContext(_options))
        {
            context.Users.Add(originalUser);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var updatedUser = new User("n@x.com", "New") { Id = id };
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
            var savedUser = await context.Users.FindAsync([id], TestContext.Current.CancellationToken);
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
        var u = new User("e@x.com", "Name") { Id = Guid.NewGuid() };
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
        var id = Guid.NewGuid();
        await using (var context = new ApplicationDbContext(_options))
        {
            var user = new User("d@x.com", "Delete") { Id = id };
            context.Users.Add(user);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var context = new ApplicationDbContext(_options))
        {
            var repo = new UserRepository(context, _loggerMock.Object);
            var result = await repo.DeleteAsync(id);
            Assert.True(result);
            var deleted = await context.Users.FindAsync([id], TestContext.Current.CancellationToken);
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