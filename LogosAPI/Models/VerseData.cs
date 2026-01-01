namespace LogosAPI.Models;

/// <summary>
/// Represents verse data containing tokens
/// </summary>
public sealed class VerseData
{
    public required List<TokenData> Tokens { get; init; }
}
