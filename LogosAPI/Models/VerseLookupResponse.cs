namespace LogosAPI.Models;

/// <summary>
/// Response model for verse lookup operations
/// </summary>
public sealed class VerseLookupResponse
{
    /// <summary>
    /// Result message
    /// </summary>
    public required string Message { get; init; }
    
    /// <summary>
    /// Requested verse references
    /// </summary>
    public required string[] VerseReferences { get; init; }
}
