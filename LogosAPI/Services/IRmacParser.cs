namespace LogosAPI.Services;

using LogosAPI.Models;

/// <summary>
/// Interface for parsing Robinson's Morphological Analysis Codes (RMAC)
/// </summary>
public interface IRmacParser
{
    /// <summary>
    /// Parses an RMAC code into its morphological components
    /// </summary>
    /// <param name="rmacCode">The RMAC code (e.g., "V-AAI-3S", "N-GSM-P")</param>
    /// <returns>Parsed morphology info, or null if parsing fails</returns>
    MorphologyInfo? Parse(string? rmacCode);
    
    /// <summary>
    /// Attempts to parse an RMAC code without returning null for empty inputs
    /// </summary>
    /// <param name="rmacCode">The RMAC code</param>
    /// <param name="result">The parsed result if successful</param>
    /// <returns>True if parsing succeeded, false otherwise</returns>
    bool TryParse(string? rmacCode, out MorphologyInfo? result);
}
