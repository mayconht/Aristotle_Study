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

        // Creating an exception switch to handle different exception types
        // this is quite interesting because it allows us to handle different exceptions in a more structured way
        // and return appropriate error responses based on the exception type.
        var errorResponse = exception switch
        {
            // After a while I sterted to think that this is not the best way to handle exceptions
            // because it can lead to a lot of boilerplate code and can be hard to maintain

            UserNotFoundException userNotFound => new ErrorResponse
            {
                Type = "UserNotFound",
                Title = "User Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = userNotFound.Message,
                Instance = context.Request.Path,
                ErrorCode = userNotFound.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["entityType"] = userNotFound.EntityType,
                    ["entityId"] = userNotFound.EntityId
                }
            },

            EntityNotFoundException entityNotFound => new ErrorResponse
            {
                Type = "EntityNotFound",
                Title = "Entity Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = entityNotFound.Message,
                Instance = context.Request.Path,
                ErrorCode = entityNotFound.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["entityType"] = entityNotFound.EntityType,
                    ["entityId"] = entityNotFound.EntityId
                }
            },

            DomainValidationException validationException => new ErrorResponse
            {
                Type = "ValidationFailed",
                Title = "Validation Failed",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = validationException.Message,
                Instance = context.Request.Path,
                ErrorCode = validationException.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["validationErrors"] = validationException.ValidationErrors,
                    ["targetType"] = validationException.TargetType
                }
            },

            DuplicateUserEmailException duplicateEmail => new ErrorResponse
            {
                Type = "DuplicateEmail",
                Title = "Duplicate Email",
                Status = (int)HttpStatusCode.Conflict,
                Detail = duplicateEmail.Message,
                Instance = context.Request.Path,
                ErrorCode = duplicateEmail.ErrorCode
            },


            DomainException domainException => new ErrorResponse
            {
                Type = "DomainError",
                Title = "Domain Error",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = domainException.Message,
                Instance = context.Request.Path,
                ErrorCode = domainException.ErrorCode
            },


            ServiceOperationException serviceOperation => new ErrorResponse
            {
                Type = "ServiceOperationFailed",
                Title = "Service Operation Failed",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Instance = context.Request.Path,
                ErrorCode = serviceOperation.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["service"] = serviceOperation.Service,
                    ["operation"] = serviceOperation.Operation
                }
            },

            ApplicationException applicationException => new ErrorResponse
            {
                Type = "ApplicationError",
                Title = "Application Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Instance = context.Request.Path,
                ErrorCode = applicationException.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["service"] = applicationException.Service
                }
            },

            RepositoryException repositoryException => new ErrorResponse
            {
                Type = "RepositoryError",
                Title = "Data Access Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An error occurred while accessing data",
                Instance = context.Request.Path,
                ErrorCode = repositoryException.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["repositoryType"] = repositoryException.RepositoryType,
                    ["entityId"] = repositoryException.EntityId
                }
            },

            DatabaseException databaseException => new ErrorResponse
            {
                Type = "DatabaseError",
                Title = "Database Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Instance = context.Request.Path,
                ErrorCode = databaseException.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["operation"] = databaseException.Operation,
                    ["tableName"] = databaseException.TableName
                }
            },

            InfrastructureException infrastructureException => new ErrorResponse
            {
                Type = "InfrastructureError",
                Title = "Infrastructure Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = RequestDetailsMessage,
                Instance = context.Request.Path,
                ErrorCode = infrastructureException.ErrorCode,
                Extensions = new Dictionary<string, object?>
                {
                    ["component"] = infrastructureException.Component
                }
            },

            ArgumentNullException argumentNull => new ErrorResponse
            {
                Type = "InvalidArgument",
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argumentNull.Message,
                Instance = context.Request.Path,
                ErrorCode = "ARGUMENT_NULL",
                Extensions = new Dictionary<string, object?>
                {
                    ["parameterName"] = argumentNull.ParamName
                }
            },

            ArgumentException argumentException => new ErrorResponse
            {
                Type = "InvalidArgument",
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argumentException.Message,
                Instance = context.Request.Path,
                ErrorCode = "ARGUMENT_INVALID",
                Extensions = new Dictionary<string, object?>
                {
                    ["parameterName"] = argumentException.ParamName
                }
            },

            _ => new ErrorResponse
            {
                Type = "InternalServerError",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred. Please try again later.",
                Instance = context.Request.Path,
                ErrorCode = "UNKNOWN_ERROR"
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