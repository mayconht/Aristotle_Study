namespace Aristotle.Infrastructure.Exceptions;

/// <summary>
/// Base exception class for infrastructure-related exceptions.
/// This class represents failures in external systems, data access layers,
/// and other infrastructure concerns that are not part of the core domain logic.
/// </summary>
public abstract class InfrastructureException : Exception
{
    /// <summary>
    /// Gets the error code associated with this infrastructure exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the component or service that caused this exception.
    /// </summary>
    public string? Component { get; }

    /// <summary>
    /// Initializes a new instance of the InfrastructureException class with a component name.
    /// </summary>
    /// <param name="component">The component or service that caused this exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    protected InfrastructureException(string component, string message) : base(message)
    {
        ErrorCode = GetType().Name;
        Component = component;
    }
}