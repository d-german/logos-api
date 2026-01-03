namespace LogosAPI.Models;

/// <summary>
/// Represents Strong's Concordance information for a word
/// </summary>
/// <param name="Number">Strong's Concordance number (e.g., G976, H1234)</param>
/// <param name="Definition">Strong's definition of the word (may be null if not available)</param>
public sealed record StrongsInfo(
    string Number,
    string? Definition
);
