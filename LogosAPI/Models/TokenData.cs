using System.Text.Json.Serialization;

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
    
    /// <summary>
    /// Robinson's Morphological Analysis Code description (may be null if not in JSON)
    /// </summary>
    [JsonPropertyName("rmac_desc")]
    public string? RmacDesc { get; init; }
}
