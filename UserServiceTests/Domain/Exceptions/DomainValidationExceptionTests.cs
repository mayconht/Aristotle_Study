using Aristotle.Domain.Exceptions;
using Xunit;

namespace UserService.UnitTests.Domain.Exceptions;

public class DomainValidationExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var errors = new Dictionary<string, List<string>>
        {
            { "Name", new List<string> { "Required" } },
            { "Email", new List<string> { "Invalid" } }
        };
        var targetType = "User";

        // Act
        var exception = new DomainValidationException(errors, targetType);

        // Assert
        Assert.Equal($"Validation failed for {targetType}.", exception.Message);
        Assert.Equal("DomainValidationException", exception.ErrorCode);
        Assert.Equal(targetType, exception.TargetType);
        Assert.Same(errors, exception.ValidationErrors);
    }
}