namespace Aristotle.Domain.Models;

/// <summary>
/// Response model for API error messages
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Short, human-readable summary of the error
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Detailed error explanation for API consumers
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// Additional information about the error
    /// </summary>
    public Dictionary<string, object?>? Extensions { get; set; }
}
