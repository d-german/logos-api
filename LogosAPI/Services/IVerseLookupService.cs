using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service for looking up Bible verses with enriched lexicon data
/// </summary>
public interface IVerseLookupService
{
    /// <summary>
    /// Looks up verses by their references, normalizes input, and enriches with lexicon entries
    /// </summary>
    /// <param name="references">Verse references in any supported format (e.g., "Matt 1:1", "John 3:16")</param>
    /// <returns>Result containing found verses with tokens/lexicon and list of not-found references</returns>
    VerseLookupResult LookupVerses(IEnumerable<string> references);
}
