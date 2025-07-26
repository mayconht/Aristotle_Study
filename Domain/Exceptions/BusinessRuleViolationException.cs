namespace Aristotle.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated in the domain.
/// This exception represents violations of domain-specific business logic that should not be allowed.
/// Examples: Attempting to create duplicate entities, invalid state transitions, etc.
/// </summary>
[Serializable]
public class BusinessRuleViolationException : DomainException
{
    /// <summary>
    /// Gets the name of the business rule that was violated.
    /// </summary>
    public string RuleName { get; }

    /// <summary>
    /// Gets additional context information about the violation.
    /// </summary>
    public Dictionary<string, object> Context { get; }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class.
    /// </summary>
    public BusinessRuleViolationException() : base("A business rule was violated.")
    {
        RuleName = "Unknown";
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class with a rule name.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    public BusinessRuleViolationException(string ruleName)
        : base($"Business rule '{ruleName}' was violated.")
    {
        RuleName = ruleName;
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class with a rule name and custom message.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    /// <param name="message">Custom error message describing the violation.</param>
    public BusinessRuleViolationException(string ruleName, string message)
        : base(message)
    {
        RuleName = ruleName;
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class with rule name, message and context.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    /// <param name="message">Custom error message describing the violation.</param>
    /// <param name="context">Additional context information about the violation.</param>
    public BusinessRuleViolationException(string ruleName, string message, Dictionary<string, object> context)
        : base(message)
    {
        RuleName = ruleName;
        Context = context;
    }

    /// <summary>
    /// Initializes a new instance of the BusinessRuleViolationException class with an inner exception.
    /// </summary>
    /// <param name="ruleName">The name of the business rule that was violated.</param>
    /// <param name="message">Custom error message describing the violation.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BusinessRuleViolationException(string ruleName, string message, Exception innerException)
        : base(message, innerException)
    {
        RuleName = ruleName;
        Context = new Dictionary<string, object>();
    }

    /// <summary>
    /// Adds context information to the exception.
    /// </summary>
    /// <param name="key">The context key.</param>
    /// <param name="value">The context value.</param>
    public void AddContext(string key, object value)
    {
        Context[key] = value;
    }
}