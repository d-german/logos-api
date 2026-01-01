namespace LogosAPI.Models;

/// <summary>
/// Represents a single token (word) in a verse
/// </summary>
public sealed class TokenData
{
    public required string Gloss { get; init; }
    public required string Greek { get; init; }
    public required string Translit { get; init; }
    public required string Strongs { get; init; }
    public required string Rmac { get; init; }
    public required string RmacDesc { get; init; }
}
