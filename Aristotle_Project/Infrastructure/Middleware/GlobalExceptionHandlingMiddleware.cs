using Aristotle.Domain.Exceptions;
using Aristotle.Infrastructure.Exceptions;
using Aristotle.Application.Exceptions;
using System.Net;
using System.Text.Json;
using ApplicationException = Aristotle.Application.Exceptions.ApplicationException;

namespace Aristotle.Infrastructure.Middleware;

/// <summary>
/// Global exception handling middleware that catches and transforms exceptions
/// into appropriate HTTP responses following hexagonal architecture principles.
/// This middleware provides consistent error responses across the entire application.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private const string RequestDetailsMessage = "An error occurred while processing your request";
    private const string ErrorCode = "errorCode";
    private const string EntityId = "entityId";
    private const string EntityType = "entityType";
    private const string Service = "service";
    private const string Operation = "operation";
    private const string RepositoryType = "repositoryType";
    private const string ParameterName = "parameterName";
    private const string Component = "component";
    private const string TableName = "tableName";
    private const string ValidationErrors = "validationErrors";
    private const string TargetType = "targetType";
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

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
            _logger.LogError(ex, "An unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions by converting them to appropriate HTTP responses.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <param name="exception">The exception that occurred.</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            UserNotFoundException userNotFound => new ErrorResponse
            {
                Title = "User Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = userNotFound.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [EntityType] = userNotFound.EntityType,
                    [EntityId] = userNotFound.EntityId,
                    [ErrorCode] = userNotFound.ErrorCode
                }
            },

            EntityNotFoundException entityNotFound => new ErrorResponse
            {
                Title = "Entity Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = entityNotFound.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [EntityType] = entityNotFound.EntityType,
                    [EntityId] = entityNotFound.EntityId,
                    [ErrorCode] = entityNotFound.ErrorCode
                }
            },

            DomainValidationException validationException => new ErrorResponse
            {
                Title = "Validation Failed",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = validationException.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [ValidationErrors] = validationException.ValidationErrors,
                    [TargetType] = validationException.TargetType,
                    [ErrorCode] = validationException.ErrorCode
                }
            },

            DuplicateUserEmailException duplicateEmail => new ErrorResponse
            {
                Title = "Duplicate Email",
                Status = (int)HttpStatusCode.Conflict,
                Detail = duplicateEmail.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [ErrorCode] = duplicateEmail.ErrorCode
                }
            },

            DomainException domainException => new ErrorResponse
            {
                Title = "Domain Error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = domainException.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [ErrorCode] = domainException.ErrorCode
                }
            },

            ServiceOperationException serviceOperation => new ErrorResponse
            {
                Title = "Service Operation Failed",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Extensions = new Dictionary<string, object?>
                {
                    [Service] = serviceOperation.Service,
                    [Operation] = serviceOperation.Operation,
                    [ErrorCode] = serviceOperation.ErrorCode
                }
            },

            ApplicationException applicationException => new ErrorResponse
            {
                Title = "Application Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Extensions = new Dictionary<string, object?>
                {
                    [Service] = applicationException.Service,
                    [ErrorCode] = applicationException.ErrorCode
                }
            },

            RepositoryException repositoryException => new ErrorResponse
            {
                Title = "Data Access Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An error occurred while accessing data",
                Extensions = new Dictionary<string, object?>
                {
                    [RepositoryType] = repositoryException.RepositoryType,
                    [EntityId] = repositoryException.EntityId,
                    [ErrorCode] = repositoryException.ErrorCode
                }
            },

            DatabaseException databaseException => new ErrorResponse
            {
                Title = "Database Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Extensions = new Dictionary<string, object?>
                {
                    [Operation] = databaseException.Operation,
                    [TableName] = databaseException.TableName,
                    [ErrorCode] = databaseException.ErrorCode
                }
            },

            InfrastructureException infrastructureException => new ErrorResponse
            {
                Title = "Infrastructure Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Extensions = new Dictionary<string, object?>
                {
                    [Component] = infrastructureException.Component,
                    [ErrorCode] = infrastructureException.ErrorCode
                }
            },

            ArgumentNullException argumentNull => new ErrorResponse
            {
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argumentNull.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [ParameterName] = argumentNull.ParamName,
                    [ErrorCode] = "ARGUMENT_NULL"
                }
            },

            ArgumentException argumentException => new ErrorResponse
            {
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argumentException.Message,
                Extensions = new Dictionary<string, object?>
                {
                    [ParameterName] = argumentException.ParamName,
                    [ErrorCode] = "ARGUMENT_INVALID"
                }
            },

            _ => new ErrorResponse
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred. Please try again later.",
                Extensions = new Dictionary<string, object?>
                {
                    [ErrorCode] = "UNKNOWN_ERROR"
                }
            }
        };

        response.StatusCode = errorResponse.Status;

        switch (exception)
        {
            case DomainException:
            case ApplicationException:
                _logger.LogWarning(exception, "Business logic exception occurred: {ExceptionType}",
                    exception.GetType().Name);
                break;
            case InfrastructureException:
                _logger.LogError(exception, "Infrastructure exception occurred: {ExceptionType}",
                    exception.GetType().Name);
                break;
            case ArgumentException:
                _logger.LogWarning(exception, "Argument exception occurred: {ExceptionType}", exception.GetType().Name);
                break;
            default:
                _logger.LogError(exception, "Unhandled exception occurred: {ExceptionType}", exception.GetType().Name);
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, JsonOptions);

        await response.WriteAsync(jsonResponse);
    }
}