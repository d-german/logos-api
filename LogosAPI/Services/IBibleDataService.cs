using System.Collections.Concurrent;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Interface for Bible data service
/// Provides access to verses and lexicon data
/// </summary>
public interface IBibleDataService
{
    /// <summary>
    /// Gets the verses dictionary (VerseReference -> VerseData)
    /// </summary>
    ConcurrentDictionary<string, VerseData> Verses { get; }

    /// <summary>
    /// Gets the lexicon dictionary (Strong's Number -> Definition)
    /// </summary>
    ConcurrentDictionary<string, string> Lexicon { get; }

    /// <summary>
    /// Gets the total number of verses loaded
    /// </summary>
    int VersesCount { get; }

    /// <summary>
    /// Gets the total number of lexicon entries loaded
    /// </summary>
    int LexiconCount { get; }

    /// <summary>
    /// Checks if the service is initialized
    /// </summary>
    bool IsInitialized { get; }
}
