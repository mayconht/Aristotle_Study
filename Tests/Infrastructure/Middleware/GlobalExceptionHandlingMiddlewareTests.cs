using Aristotle.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace Aristotle.UnitTests.Infrastructure.Middleware;

public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<GlobalExceptionHandlingMiddleware>> _mockLogger;
    private readonly GlobalExceptionHandlingMiddleware _middleware;

    public GlobalExceptionHandlingMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        _middleware = new GlobalExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var middleware = new GlobalExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(middleware);
    }

    [Fact]
    public void Constructor_WithNullNext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception =
            Assert.Throws<ArgumentNullException>(() =>
                new GlobalExceptionHandlingMiddleware(null!, _mockLogger.Object));

        Assert.Equal("next", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var exception =
            Assert.Throws<ArgumentNullException>(() => new GlobalExceptionHandlingMiddleware(_mockNext.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    #endregion


    //From now tests feels that falls a bit with the integration tests
    // When I implement integration tests, I might remove these or comment here to explain.

    #region InvokeAsync Tests

    [Fact]
    public async Task InvokeAsync_WithNoException_ShouldCallNextMiddleware()
    {
        // Arrange
        var context = CreateHttpContext();
        _mockNext.Setup(x => x(context)).Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _mockNext.Verify(x => x(context), Times.Once);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithGenericException_ShouldReturnInternalServerError()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        VerifyLoggerWasCalled(LogLevel.Error);
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new ArgumentException("Invalid argument", "testParam");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        VerifyLoggerWasCalled(LogLevel.Warning);
    }

    [Fact]
    public async Task InvokeAsync_WithArgumentNullException_ShouldReturnBadRequest()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new ArgumentNullException("testParam", "Test parameter is null");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        VerifyLoggerWasCalled(LogLevel.Warning);
    }

    [Fact]
    public async Task InvokeAsync_WithUnauthorizedAccessException_ShouldReturnUnauthorizedError()
    {
        // Another cool test, because UnauthorizedAccessException was throwing 500 instead of 401
        // So another bug fixed with a test to prove it.

        // Arrange
        var context = CreateHttpContext();
        var exception = new UnauthorizedAccessException("Access denied");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        VerifyLoggerWasCalled(LogLevel.Warning);
    }

    [Fact]
    public async Task InvokeAsync_WithNotSupportedException_ShouldReturnInternalServerError()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new NotSupportedException("Operation not supported");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        VerifyLoggerWasCalled(LogLevel.Error);
    }

    [Fact]
    public async Task InvokeAsync_WithTimeoutException_ShouldReturnRequestTimeOut()
    {
        //I think this is collest test I ever made.
        // It is simple, but fixed a bad implementation in the middleware I made before.

        // Arrange
        var context = CreateHttpContext();
        var exception = new TimeoutException("Request timeout");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status408RequestTimeout, context.Response.StatusCode);
        VerifyLoggerWasCalled(LogLevel.Error);
    }

    #endregion

    #region Response Content Tests

    [Fact]
    public async Task InvokeAsync_WithException_ShouldReturnJsonResponse()
    {
        // Arrange
        var context = CreateHttpContext();
        var exception = new Exception("Test exception");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var content = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        Assert.NotEmpty(content);

        var jsonDocument = JsonDocument.Parse(content);
        Assert.NotNull(jsonDocument);
    }

    #endregion

    #region Helper Methods

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
        return context;
    }

    private void VerifyLoggerWasCalled(LogLevel expectedLevel)
    {
        _mockLogger.Verify(
            x => x.Log(
                expectedLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion
}