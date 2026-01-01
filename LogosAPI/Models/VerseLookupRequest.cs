namespace LogosAPI.Models;

/// <summary>
/// Request model for verse lookup operations
/// </summary>
public sealed class VerseLookupRequest
{
    /// <summary>
    /// Array of verse references (e.g., "Matt.1.1", "John.3.16")
    /// </summary>
    public required string[] VerseReferences { get; init; }
}
