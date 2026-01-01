namespace LogosAPI.Models;

/// <summary>
/// Represents a Bible verse with its reference and enriched tokens
/// </summary>
/// <param name="Reference">Canonical verse reference (e.g., Matt.1.1)</param>
/// <param name="Tokens">List of tokens (words) with lexicon entries</param>
public sealed record VerseResponse(
    string Reference,
    IReadOnlyList<TokenResponse> Tokens
);
