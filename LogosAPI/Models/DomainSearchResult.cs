namespace LogosAPI.Models;

/// <summary>
/// Response model for a semantic domain search result
/// </summary>
public sealed class DomainSearchResult
{
    /// <summary>
    /// The major Louw-Nida domain number (e.g., 57 for "Possess, Transfer, Exchange")
    /// </summary>
    public required int MajorDomain { get; init; }
    
    /// <summary>
    /// Human-readable label for the domain
    /// </summary>
    public required string DomainLabel { get; init; }
    
    /// <summary>
    /// Total number of word entries in this domain
    /// </summary>
    public required int TotalEntries { get; init; }
    
    /// <summary>
    /// List of words/lemmas in this domain, sorted by frequency descending
    /// </summary>
    public required IReadOnlyList<DomainEntry> Entries { get; init; }
}
