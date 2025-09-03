using Aristotle.Domain.Exceptions;
using Xunit;

namespace Aristotle.UnitTests.Domain.Exceptions;

public class ErrorResponseTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var response = new ErrorResponse();

        // Assert
        Assert.NotNull(response.Title);
        Assert.Equal(string.Empty, response.Title);
        Assert.Equal(0, response.Status);
        Assert.NotNull(response.Detail);
        Assert.Equal(string.Empty, response.Detail);
        Assert.NotNull(response.Extensions);
        Assert.Empty(response.Extensions);
    }

    [Fact]
    public void Properties_ShouldBeSetAndRetrieved()
    {
        // Arrange
        var response = new ErrorResponse
        {
            Title = "Error occured",
            Status = 404,
            Detail = "Item not found",
            Extensions = new Dictionary<string, object?>
            {
                { "traceId", "12345" },
                { "timestamp", null }
            }
        };

        // Act & Assert
        Assert.Equal("Error occured", response.Title);
        Assert.Equal(404, response.Status);
        Assert.Equal("Item not found", response.Detail);
        Assert.Equal(2, response.Extensions.Count);
        Assert.Equal("12345", response.Extensions["traceId"]);
        Assert.Null(response.Extensions["timestamp"]);
    }
}