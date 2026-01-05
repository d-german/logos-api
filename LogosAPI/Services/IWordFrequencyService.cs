namespace LogosAPI.Services;

/// <summary>
/// Word frequency information for a Greek lemma
/// </summary>
/// <param name="Count">Total occurrences in the NT</param>
/// <param name="Rank">Frequency rank (1 = most common)</param>
/// <param name="IsHapax">True if word occurs only once in NT (hapax legomenon)</param>
public sealed record WordFrequencyInfo(int Count, int Rank, bool IsHapax);

/// <summary>
/// Service for looking up word frequency data in the Greek NT
/// </summary>
public interface IWordFrequencyService
{
    /// <summary>
    /// Gets frequency information for a Greek lemma
    /// </summary>
    /// <param name="lemma">Greek lemma (dictionary form)</param>
    /// <returns>Frequency info or null if lemma not found</returns>
    WordFrequencyInfo? GetFrequency(string? lemma);
}
