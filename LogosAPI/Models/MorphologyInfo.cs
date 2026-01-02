namespace LogosAPI.Models;

/// <summary>
/// Represents parsed morphological information from an RMAC code.
/// All properties are nullable since not all RMAC codes contain all components.
/// </summary>
public sealed record MorphologyInfo
{
    /// <summary>Part of speech (e.g., "Verb", "Noun", "Adjective", "PersonalPronoun")</summary>
    public string? Pos { get; init; }
    
    /// <summary>Verb tense (e.g., "Present", "Aorist", "Perfect", "SecondAorist")</summary>
    public string? Tense { get; init; }
    
    /// <summary>Verb voice (e.g., "Active", "Middle", "Passive", "MiddleOrPassive")</summary>
    public string? Voice { get; init; }
    
    /// <summary>Verb form: "Finite", "Participle", or "Infinitive"</summary>
    public string? VerbForm { get; init; }
    
    /// <summary>Verb mood for finite verbs only: "Indicative", "Subjunctive", "Imperative", "Optative"</summary>
    public string? Mood { get; init; }
    
    /// <summary>Grammatical case (e.g., "Nominative", "Genitive", "Dative", "Accusative", "Vocative")</summary>
    public string? Case { get; init; }
    
    /// <summary>Grammatical number: "Singular" or "Plural"</summary>
    public string? Number { get; init; }
    
    /// <summary>Grammatical gender: "Masculine", "Feminine", or "Neuter"</summary>
    public string? Gender { get; init; }
    
    /// <summary>Person for verbs and pronouns: "First", "Second", or "Third"</summary>
    public string? Person { get; init; }
    
    /// <summary>Additional flags that don't fit in standard fields (e.g., "Person", "Title", "Comparative", "Deponent")</summary>
    public IReadOnlyList<string> Flags { get; init; } = [];
}
