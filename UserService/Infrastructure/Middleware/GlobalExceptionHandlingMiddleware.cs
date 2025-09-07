using System.Net;
using System.Text.Json;
using Aristotle.Application.Exceptions;
using Aristotle.Domain.Exceptions;
using Aristotle.Infrastructure.Exceptions;
using ApplicationException = Aristotle.Application.Exceptions.ApplicationException;

namespace Aristotle.Infrastructure.Middleware;

/// <summary>
/// Global exception handling middleware that catches and transforms exceptions
/// This middleware provides error responses across the entire application.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private const string RequestDetailsMessage = "An error occurred while processing your request";

    private const string ErrorCode = "code";
    private const string ValidationErrors = "errors";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the GlobalExceptionHandlingMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">Logger for exception tracking.</param>
    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware to handle the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex is not ArgumentException)
                _logger.LogTrace(ex, "An unhandled exception occurred during request processing");

            await HandleExceptionAsync(context, ex);
        }
    }

    // TODO: Move this to shared domain project, not sure how to handle it right now.

    /// <summary>
    /// Handles exceptions by converting them to appropriate HTTP responses.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="exception">The exception that occurred.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // Group similar exceptions together for cleaner switch expression
        var errorResponse = exception switch
        {
            // Not Found exceptions
            UserNotFoundException or EntityNotFoundException => CreateErrorResponse(
                "Resource Not Found",
                HttpStatusCode.NotFound,
                exception.Message,
                exception is ADomainException domainEx ? domainEx.ErrorCode : "NOT_FOUND",
                path: context.Request.Path
            ),

            // Validation exceptions
            DomainValidationException validationException => CreateErrorResponse(
                "Validation Failed",
                HttpStatusCode.BadRequest,
                validationException.Message,
                validationException.ErrorCode,
                new Dictionary<string, object?> { [ValidationErrors] = validationException.ValidationErrors },
                context.Request.Path
            ),

            // Conflict exceptions
            DuplicateUserEmailException duplicateEmail => CreateErrorResponse(
                "Resource Conflict",
                HttpStatusCode.Conflict,
                duplicateEmail.Message,
                duplicateEmail.ErrorCode,
                path: context.Request.Path
            ),

            // Domain/Business logic exceptions
            ADomainException domainException => CreateErrorResponse(
                "Business Logic Error",
                HttpStatusCode.BadRequest,
                domainException.Message,
                domainException.ErrorCode,
                path: context.Request.Path
            ),

            // Bad request/argument exceptions
            ArgumentNullException or ArgumentException => CreateErrorResponse(
                "Invalid Request Parameter",
                HttpStatusCode.BadRequest,
                exception.Message,
                "ARGUMENT_INVALID",
                path: context.Request.Path
            ),

            // Infrastructure/internal exceptions (don't expose details)
            ServiceOperationException or
                ApplicationException or
                RepositoryException or
                DatabaseException or
                InfrastructureException => CreateErrorResponse(
                    "Internal Server Error",
                    HttpStatusCode.InternalServerError,
                    RequestDetailsMessage,
                    "INTERNAL_ERROR",
                    path: context.Request.Path
                ),

            // Timeout exceptions
            TimeoutException => CreateErrorResponse(
                "Request Timeout",
                HttpStatusCode.RequestTimeout,
                "The operation timed out. Please try again later.",
                "TIMEOUT_ERROR",
                path: context.Request.Path
            ),

            // Authorization exceptions
            UnauthorizedAccessException => CreateErrorResponse(
                "Unauthorized",
                HttpStatusCode.Unauthorized,
                "Access is denied due to invalid credentials.",
                "UNAUTHORIZED",
                path: context.Request.Path
            ),

            // Default case
            _ => CreateErrorResponse(
                "Internal Server Error",
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred. Please try again later.",
                "UNKNOWN_ERROR",
                path: context.Request.Path
            )
        };

        response.StatusCode = errorResponse.Status;

        // Log at appropriate levels based on exception type
        //The test also helped me to create a better trace in case we need it.
        //TODO maybe on future we can use API versioning on this right? Tags? headers?
        switch (exception)
        {
            case UserEmailNotFoundException:
            case UserNotFoundException:
                _logger.LogTrace(exception, "Information not found: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                break;
            case ADomainException:
                _logger.LogWarning("Domain validation error: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                _logger.LogTrace(exception, "Domain validation error: {Message} | Path: {Path}",
                    exception.Message,
                    context.Request.Path);
                break;
            case ApplicationException:
                _logger.LogError(exception, "Application error: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                break;
            case ArgumentException:
                _logger.LogTrace(exception, "Invalid argument: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                _logger.LogWarning("Invalid argument:{Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                break;
            case TimeoutException:
                _logger.LogError(exception, "Operation timed out");
                break;
            case UnauthorizedAccessException:
                _logger.LogTrace(exception, "Unauthorized access attempt: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                _logger.LogWarning("Unauthorized access attempt: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                break;
            default:
                _logger.LogError(exception, "Unexpected error: {Message} | Path: {Path}", exception.Message,
                    context.Request.Path);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);
        await response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Creates a standardized error response object
    /// </summary>
    private static ErrorResponse CreateErrorResponse(string title, HttpStatusCode status, string detail,
        string code, Dictionary<string, object?>? extensions = null, string? path = null)
    {
        var errorResponseExtensions = extensions ?? new Dictionary<string, object?>();
        errorResponseExtensions[ErrorCode] = code;
        if (!string.IsNullOrEmpty(path)) errorResponseExtensions["path"] = path;

        return new ErrorResponse
        {
            Title = title,
            Status = (int)status,
            Detail = detail,
            Extensions = errorResponseExtensions
        };
    }
}

// TODO: Move this to shared domain project.
/// <summary>
/// Response model for API error messages
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Short, human-readable summary of the error
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Detailed error explanation for API consumers
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Additional information about the error
    /// </summary>
    public Dictionary<string, object?>? Extensions { get; set; }
}