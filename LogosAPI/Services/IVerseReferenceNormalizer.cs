namespace LogosAPI.Services;

/// <summary>
/// Interface for normalizing Bible verse references
/// </summary>
public interface IVerseReferenceNormalizer
{
    /// <summary>
    /// Normalizes a verse reference to canonical format (Book.Chapter.Verse)
    /// </summary>
    /// <param name="input">Input reference in any supported format</param>
    /// <returns>Normalized reference</returns>
    /// <exception cref="ArgumentException">If input cannot be normalized</exception>
    string Normalize(string input);

    /// <summary>
    /// Attempts to normalize a verse reference without throwing
    /// </summary>
    bool TryNormalize(string input, out string? normalized);

    /// <summary>
    /// Checks if input can be normalized to a valid reference
    /// </summary>
    bool IsValid(string input);
}
