using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Interface for fronting data service
/// Provides access to Greek word order fronting information
/// </summary>
public interface IFrontingDataService
{
    /// <summary>
    /// Gets fronting information for a specific verse
    /// </summary>
    /// <param name="verseReference">Canonical verse reference (e.g., John.1.1)</param>
    /// <returns>FrontingInfo if fronting data exists, null otherwise</returns>
    FrontingInfo? GetFronting(string verseReference);

    /// <summary>
    /// Checks if a verse has fronting data
    /// </summary>
    /// <param name="verseReference">Canonical verse reference (e.g., John.1.1)</param>
    /// <returns>True if the verse has fronting, false otherwise</returns>
    bool HasFronting(string verseReference);

    /// <summary>
    /// Gets the total number of verses with fronting data
    /// </summary>
    int FrontingCount { get; }

    /// <summary>
    /// Checks if the service is initialized
    /// </summary>
    bool IsInitialized { get; }
}
