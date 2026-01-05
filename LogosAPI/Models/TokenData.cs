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
    
    /// <summary>
    /// Strong's definition of the word (may be null if not in JSON)
    /// </summary>
    [JsonPropertyName("strong_def")]
    public string? StrongDef { get; init; }
    
    /// <summary>
    /// Dictionary/base form of the word (e.g., λόγος for λόγον)
    /// </summary>
    [JsonPropertyName("lemma")]
    public string? Lemma { get; init; }
    
    /// <summary>
    /// Louw-Nida semantic domain code (e.g., "033005")
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }
    
    /// <summary>
    /// Louw-Nida section number (e.g., "33.38")
    /// </summary>
    [JsonPropertyName("louw_nida")]
    public string? LouwNida { get; init; }
    
    /// <summary>
    /// Syntactic role: s=subject, o=object, p=predicate, vc=verb-copula
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; init; }
    
    /// <summary>
    /// Word type: common, proper, personal, etc.
    /// </summary>
    [JsonPropertyName("word_type")]
    public string? WordType { get; init; }
    
    /// <summary>
    /// Pronoun referent - xml:id of the antecedent this word refers to
    /// </summary>
    [JsonPropertyName("referent")]
    public string? Referent { get; init; }
}
