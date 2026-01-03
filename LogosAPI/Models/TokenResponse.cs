namespace LogosAPI.Models;

/// <summary>
/// Represents a token (word) with its linguistic data
/// </summary>
/// <param name="Gloss">English translation/meaning of the word</param>
/// <param name="Greek">Original Greek text</param>
/// <param name="Translit">Transliteration of the Greek</param>
/// <param name="Strongs">Strong's Concordance number (e.g., G976)</param>
/// <param name="Rmac">Robinson's Morphological Analysis Code</param>
/// <param name="RmacDesc">Human-readable morphology description</param>
/// <param name="Morph">Parsed morphological components from the RMAC code</param>
/// <param name="StrongDef">Strong's definition of the word</param>
public sealed record TokenResponse(
    string Gloss,
    string Greek,
    string Translit,
    string Strongs,
    string Rmac,
    string? RmacDesc,
    MorphologyInfo? Morph,
    string? StrongDef
);
