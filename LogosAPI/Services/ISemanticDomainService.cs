using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service for searching words by Louw-Nida semantic domain
/// </summary>
public interface ISemanticDomainService
{
    /// <summary>
    /// Gets all words in a major semantic domain.
    /// </summary>
    /// <param name="majorDomain">The major domain number (e.g., 57 for "Possess, Transfer, Exchange")</param>
    /// <param name="maxVersesPerWord">Maximum sample verses to include per word (default: 3)</param>
    /// <returns>Domain search result with all entries, or null if domain not found</returns>
    DomainSearchResult? GetWordsByDomain(int majorDomain, int maxVersesPerWord = 3);
    
    /// <summary>
    /// Gets words related to a specific Louw-Nida number within the same major domain.
    /// Useful for finding semantically related words (e.g., all economic terms related to "steal").
    /// </summary>
    /// <param name="louwNidaNumber">The L-N number (e.g., "57.235")</param>
    /// <param name="limit">Maximum related words to return (default: 10)</param>
    /// <param name="maxVersesPerWord">Maximum sample verses per word (default: 3)</param>
    /// <returns>Related words result, or null if L-N number not found</returns>
    RelatedWordsResult? GetRelatedWords(string louwNidaNumber, int limit = 10, int maxVersesPerWord = 3);
    
    /// <summary>
    /// Gets all available major domain numbers in the data.
    /// </summary>
    /// <returns>Sorted list of major domain numbers (1-93)</returns>
    IReadOnlyList<int> GetAvailableDomains();
}
