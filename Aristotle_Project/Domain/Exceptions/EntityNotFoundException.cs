namespace Aristotle.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found in the domain.
/// This represents a business rule violation where a required entity does not exist.
/// </summary>
public class EntityNotFoundException : DomainException
{
    /// <summary>
    /// Gets the type of entity that was not found.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the identifier that was used to search for the entity.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the EntityNotFoundException class.
    /// </summary>
    public EntityNotFoundException() : base("The requested entity was not found.")
    {
        EntityType = "Unknown";
    }

    /// <summary>
    /// Initializes a new instance of the EntityNotFoundException class for a specific entity type.
    /// </summary>
    /// <param name="entityType">The type of entity that was not found.</param>
    public EntityNotFoundException(string entityType)
        : base($"The requested {entityType} was not found.")
    {
        EntityType = entityType;
    }

    /// <summary>
    /// Initializes a new instance of the EntityNotFoundException class for a specific entity type and identifier.
    /// </summary>
    /// <param name="entityType">The type of entity that was not found.</param>
    /// <param name="entityId">The identifier that was used to search for the entity.</param>
    public EntityNotFoundException(string entityType, object entityId)
        : base($"The {entityType} with identifier '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the EntityNotFoundException class with a custom message.
    /// </summary>
    /// <param name="entityType">The type of entity that was not found.</param>
    /// <param name="entityId">The identifier that was used to search for the entity.</param>
    /// <param name="message">Custom error message.</param>
    public EntityNotFoundException(string entityType, object entityId, string message)
        : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the EntityNotFoundException class with an inner exception.
    /// </summary>
    /// <param name="entityType">The type of entity that was not found.</param>
    /// <param name="entityId">The identifier that was used to search for the entity.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>W
    public EntityNotFoundException(string entityType, object entityId, Exception innerException)
        : base($"The {entityType} with identifier '{entityId}' was not found.", innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}