namespace Aristotle.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when database operations fail.
/// This includes connection issues, query failures, transaction problems, and other database-related errors.
/// </summary>
[Serializable]
public class DatabaseException : InfrastructureException
{
    /// <summary>
    /// Gets the database operation that failed.
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// Gets the table or entity involved in the failed operation.
    /// </summary>
    public string? TableName { get; }

    /// <summary>
    /// Gets the SQL state or error code from the database provider.
    /// </summary>
    public string? SqlState { get; }

    /// <summary>
    /// Initializes a new instance of the DatabaseException class with operation details.
    /// </summary>
    /// <param name="operation">The database operation that failed.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DatabaseException(string operation, string message) : base("Database", message)
    {
        Operation = operation;
    }


    /// <summary>
    /// Initializes a new instance of the DatabaseException class with full details.
    /// </summary>
    /// <param name="operation">The database operation that failed.</param>
    /// <param name="tableName">The table or entity involved in the failed operation.</param>
    /// <param name="sqlState">The SQL state or error code from the database provider.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DatabaseException(string operation, string tableName, string sqlState, string message) : base("Database",
        message)
    {
        Operation = operation;
        TableName = tableName;
        SqlState = sqlState;
    }
}

/// <summary>
/// Exception thrown when a repository operation fails.
/// This includes entity not found during updates, constraint violations, and other repository-specific errors.
/// </summary>
[Serializable]
public class RepositoryException : DatabaseException
{
    /// <summary>
    /// Gets the repository type that caused the exception.
    /// </summary>
    public string? RepositoryType { get; }

    /// <summary>
    /// Gets the entity identifier that was involved in the operation.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class with entity details.
    /// </summary>
    /// <param name="repositoryType">The repository type that caused the exception.</param>
    /// <param name="entityId">The entity identifier involved in the operation.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public RepositoryException(string repositoryType, object entityId, string message) : base("Repository", message)
    {
        RepositoryType = repositoryType;
        EntityId = entityId;
    }
}