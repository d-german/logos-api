namespace LogosAPI.Models;

/// <summary>
/// Complete result of a verse lookup operation
/// </summary>
/// <param name="Verses">Successfully found verses with enriched token data</param>
/// <param name="NotFound">List of verse references that could not be found or normalized</param>
public sealed record VerseLookupResult(
    IReadOnlyList<VerseResponse> Verses,
    IReadOnlyList<string> NotFound
);
