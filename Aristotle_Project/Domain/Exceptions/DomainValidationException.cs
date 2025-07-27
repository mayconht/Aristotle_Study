namespace Aristotle.Domain.Exceptions;

// Many domain exceptions are used to represent specific business rule violations or validation failures.
// Sometimes we will have more exceptions than we need, but this is okay as it allows us to be very specific about the error.
// Specialy when we are dealing with domain driven or schema driven design.

/// <summary>
/// Exception thrown when validation fails in the domain layer.
/// This exception is used when entity properties or domain operations fail validation checks.
/// It provides detailed information about which validation rules were violated.
/// </summary>
[Serializable]
public class DomainValidationException : DomainException
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
    /// Initializes a new instance of the DomainValidationException class.
    /// </summary>
    public DomainValidationException() : base("Validation failed.")
    {
        ValidationErrors = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Initializes a new instance of the DomainValidationException class with a custom message.
    /// </summary>
    /// <param name="message">Custom validation error message.</param>
    public DomainValidationException(string message) : base(message)
    {
        ValidationErrors = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Initializes a new instance of the DomainValidationException class for a specific field.
    /// </summary>
    /// <param name="fieldName">The name of the field that failed validation.</param>
    /// <param name="errorMessage">The validation error message for the field.</param>
    public DomainValidationException(string fieldName, string errorMessage)
        : base($"Validation failed for field '{fieldName}': {errorMessage}")
    {
        ValidationErrors = new Dictionary<string, List<string>>
        {
            { fieldName, [errorMessage] }
        };
    }

    /// <summary>
    /// Initializes a new instance of the DomainValidationException class with multiple validation errors.
    /// </summary>
    /// <param name="validationErrors">Dictionary containing validation errors for multiple fields.</param>
    public DomainValidationException(Dictionary<string, List<string>> validationErrors)
        : base("Multiple validation errors occurred.")
    {
        ValidationErrors = validationErrors;
    }

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

    /// <summary>
    /// Initializes a new instance of the DomainValidationException class with an inner exception.
    /// </summary>
    /// <param name="message">Custom validation error message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DomainValidationException(string message, Exception innerException) : base(message, innerException)
    {
        ValidationErrors = new Dictionary<string, List<string>>();
    }

    /// <summary>
    /// Adds a validation error for a specific field.
    /// </summary>
    /// <param name="fieldName">The name of the field that has the validation error.</param>
    /// <param name="errorMessage">The validation error message.</param>
    public void AddValidationError(string fieldName, string errorMessage)
    {
        if (!ValidationErrors.TryGetValue(fieldName, out var value))
        {
            value = [];
            ValidationErrors[fieldName] = value;
        }

        value.Add(errorMessage);
    }

    /// <summary>
    /// Gets all validation errors as a formatted string.
    /// </summary>
    /// <returns>A formatted string containing all validation errors.</returns>
    public string GetValidationErrorsAsString()
    {
        if (ValidationErrors.Count == 0)
            return "No specific validation errors available.";

        // Here we transform the dictionary into a simple list of error messages
        var errors = ValidationErrors
            .SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"))
            .ToList();

        return string.Join("; ", errors);
    }

    /// <summary>
    /// Checks if there are validation errors for a specific field.
    /// </summary>
    /// <param name="fieldName">The field name to check.</param>
    /// <returns>True if there are validation errors for the specified field.</returns>
    public bool HasErrorsForField(string fieldName)
    {
        return ValidationErrors.ContainsKey(fieldName) && ValidationErrors[fieldName].Count != 0;
    }
}