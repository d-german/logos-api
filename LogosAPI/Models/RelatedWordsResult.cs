namespace LogosAPI.Models;

/// <summary>
/// Response model for finding related words in the same semantic domain
/// </summary>
public sealed class RelatedWordsResult
{
    /// <summary>
    /// The source Louw-Nida number used for the lookup (e.g., "57.235")
    /// </summary>
    public required string SourceLouwNida { get; init; }
    
    /// <summary>
    /// The lemma of the source word
    /// </summary>
    public required string SourceLemma { get; init; }
    
    /// <summary>
    /// The major domain number containing the related words
    /// </summary>
    public required int MajorDomain { get; init; }
    
    /// <summary>
    /// Human-readable label for the domain
    /// </summary>
    public required string DomainLabel { get; init; }
    
    /// <summary>
    /// List of related words from the same semantic domain
    /// </summary>
    public required IReadOnlyList<DomainEntry> RelatedWords { get; init; }
}
