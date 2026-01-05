namespace LogosAPI.Models;

/// <summary>
/// Represents a token (word) with its linguistic data
/// </summary>
/// <param name="Gloss">English translation/meaning of the word</param>
/// <param name="Greek">Original Greek text</param>
/// <param name="Translit">Transliteration of the Greek</param>
/// <param name="Strongs">Strong's Concordance information (number and definition)</param>
/// <param name="Rmac">Robinson's Morphological Analysis Code</param>
/// <param name="RmacDesc">Human-readable morphology description</param>
/// <param name="Morph">Parsed morphological components from the RMAC code</param>
/// <param name="Lemma">Dictionary/base form of the word (e.g., λόγος for λόγον)</param>
/// <param name="Domain">Louw-Nida semantic domain code (e.g., "033005")</param>
/// <param name="DomainGloss">Human-readable domain label (e.g., "Communication")</param>
/// <param name="LouwNida">Louw-Nida section number (e.g., "33.38")</param>
/// <param name="Role">Syntactic role: s=subject, o=object, p=predicate, vc=verb-copula</param>
/// <param name="WordType">Word type: common, proper, personal, etc.</param>
/// <param name="Referent">Pronoun referent - xml:id of the antecedent this word refers to</param>
/// <param name="Frequency">Number of times this word (lemma) appears in the NT</param>
/// <param name="FrequencyRank">Frequency rank (1 = most common word in NT)</param>
/// <param name="IsHapax">True if word appears only once in the NT (hapax legomenon)</param>
public sealed record TokenResponse(
    string Gloss,
    string Greek,
    string Translit,
    StrongsInfo Strongs,
    string Rmac,
    string? RmacDesc,
    MorphologyInfo? Morph,
    string? Lemma = null,
    string? Domain = null,
    string? DomainGloss = null,
    string? LouwNida = null,
    string? Role = null,
    string? WordType = null,
    string? Referent = null,
    int? Frequency = null,
    int? FrequencyRank = null,
    bool? IsHapax = null
);
