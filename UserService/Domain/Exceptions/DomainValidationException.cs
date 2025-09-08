namespace Aristotle.Domain.Exceptions;

// Many domain exceptions are used to represent specific business rule violations or validation failures.
// Sometimes we will have more exceptions than we need, but this is okay as it allows us to be very specific about the error.
// Specialy when we are dealing with domain driven or schema driven design.

/// <summary>
/// Exception thrown when validation fails in the domain layer.
/// This exception is used when entity properties or domain operations fail validation checks.
/// It provides detailed information about which validation rules were violated.
/// </summary>
public class DomainValidationException : ADomainException
{
    /// <summary>
    /// Gets the validation errors that occurred.
    /// Key is the property or field name, Value is the list of error messages for that field.
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; }

    /// <summary>
    /// Gets the target object type that failed validation.
    /// </summary>
    public string? TargetType { get; }

    /// <summary>
    /// Initializes a new instance of the DomainValidationException class with validation errors and target type.
    /// </summary>
    /// <param name="validationErrors">Dictionary containing validation errors for multiple fields.</param>
    /// <param name="targetType">The type of object that failed validation.</param>
    public DomainValidationException(Dictionary<string, List<string>> validationErrors, string targetType)
        : base($"Validation failed for {targetType}.")
    {
        ValidationErrors = validationErrors;
        TargetType = targetType;
    }
}