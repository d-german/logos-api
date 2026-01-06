namespace LogosAPI.Models;

/// <summary>
/// Represents a word entry within a semantic domain
/// </summary>
public sealed class DomainEntry
{
    /// <summary>
    /// The subdomain number within the major domain (e.g., "235" in "57.235")
    /// </summary>
    public required string Subdomain { get; init; }
    
    /// <summary>
    /// Human-readable label for the subdomain (e.g., "Steal, Rob")
    /// </summary>
    public string? SubdomainLabel { get; init; }
    
    /// <summary>
    /// Full Louw-Nida reference number (e.g., "57.235")
    /// </summary>
    public required string LouwNida { get; init; }
    
    /// <summary>
    /// Dictionary/base form of the Greek word
    /// </summary>
    public required string Lemma { get; init; }
    
    /// <summary>
    /// English gloss/translation
    /// </summary>
    public required string Gloss { get; init; }
    
    /// <summary>
    /// Strong's concordance number (e.g., "G725")
    /// </summary>
    public required string Strongs { get; init; }
    
    /// <summary>
    /// Number of times this word (with this meaning) appears in the NT
    /// </summary>
    public required int Frequency { get; init; }
    
    /// <summary>
    /// True if word appears only once in the NT (hapax legomenon)
    /// </summary>
    public required bool IsHapax { get; init; }
    
    /// <summary>
    /// Sample verse references where this word appears (limited set)
    /// </summary>
    public required IReadOnlyList<string> SampleVerses { get; init; }
}
