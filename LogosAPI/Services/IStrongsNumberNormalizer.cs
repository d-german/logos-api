namespace LogosAPI.Services;

/// <summary>
/// Interface for normalizing Strong's Concordance numbers
/// </summary>
public interface IStrongsNumberNormalizer
{
    /// <summary>
    /// Normalizes a Strong's number to canonical format (e.g., G25, H1234)
    /// </summary>
    /// <param name="input">Input Strong's number in any supported format (e.g., G25, g25, G0025, G 25)</param>
    /// <returns>Normalized Strong's number with uppercase prefix and no leading zeros</returns>
    /// <exception cref="ArgumentException">If input cannot be normalized</exception>
    string Normalize(string input);

    /// <summary>
    /// Attempts to normalize a Strong's number without throwing
    /// </summary>
    /// <param name="input">Input Strong's number</param>
    /// <param name="normalized">The normalized Strong's number if successful, null otherwise</param>
    /// <returns>True if normalization succeeded, false otherwise</returns>
    bool TryNormalize(string input, out string? normalized);

    /// <summary>
    /// Checks if input can be normalized to a valid Strong's number
    /// </summary>
    /// <param name="input">Input to validate</param>
    /// <returns>True if input is a valid Strong's number format</returns>
    bool IsValid(string input);
}
