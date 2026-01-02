namespace LogosAPI.Models;

/// <summary>
/// Response DTO for lexicon lookup endpoint
/// </summary>
/// <param name="StrongsNumber">The normalized Strong's Concordance number (e.g., G25, H1234)</param>
/// <param name="Definition">The lexicon definition for the Strong's number</param>
public sealed record LexiconLookupResponse(
    string StrongsNumber,
    string Definition
);
