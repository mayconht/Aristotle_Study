namespace Aristotle.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found in the domain.
/// This represents a business rule violation where a required entity does not exist.
/// </summary>
public class EntityNotFoundException : ADomainException
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
    /// Initializes a new instance of the EntityNotFoundException class for a specific entity type and identifier.
    /// </summary>
    /// <param name="entityType">The type of entity that was not found.</param>
    /// <param name="entityId">The identifier that was used to search for the entity.</param>
    protected EntityNotFoundException(string entityType, object entityId)
        : base($"The {entityType} with identifier '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}