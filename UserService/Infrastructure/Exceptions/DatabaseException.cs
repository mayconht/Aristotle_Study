namespace Aristotle.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when database operations fail.
/// This includes connection issues, query failures, transaction problems, and other database-related errors.
/// </summary>
public class DatabaseException : InfrastructureException
{
    /// <summary>
    /// Gets the database operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the table or entity involved in the failed operation.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Initializes a new instance of the DatabaseException class.
    /// </summary>
    /// <param name="operation">The database operation that failed.</param>
    /// <param name="tableName">The table or entity involved in the failed operation.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="details">Additional details about the error.</param>
    public DatabaseException(string operation, string tableName, string message, string details)
        : base("Database", $"{message}. Details: {details}")
    {
        Operation = operation;
        TableName = tableName;
    }
}

/// <summary>
/// Exception thrown when repository operations fail.
/// This is a specialized version of DatabaseException for repository-level operations.
/// </summary>
public class RepositoryException : DatabaseException
{
    /// <summary>
    /// Gets the type of repository where the exception occurred.
    /// </summary>
    public string RepositoryType { get; }

    /// <summary>
    /// Gets the entity identifier associated with the failed operation.
    /// </summary>
    public object EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class with entity details.
    /// </summary>
    /// <param name="repositoryType">The type of repository where the exception occurred.</param>
    /// <param name="entityId">The entity identifier associated with the failed operation.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public RepositoryException(string repositoryType, object entityId, string message)
        : base("Repository", "Entity", message, $"Repository: {repositoryType}, EntityId: {entityId}")
    {
        RepositoryType = repositoryType;
        EntityId = entityId;
    }
}