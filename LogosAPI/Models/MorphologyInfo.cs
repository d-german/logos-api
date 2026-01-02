namespace LogosAPI.Models;

/// <summary>
/// Represents parsed morphological information from an RMAC code.
/// All properties are nullable since not all RMAC codes contain all components.
/// </summary>
public sealed record MorphologyInfo
{
    /// <summary>Part of speech (e.g., "Verb", "Noun", "Adjective")</summary>
    public string? Pos { get; init; }
    
    /// <summary>Verb tense (e.g., "Present", "Aorist", "Perfect")</summary>
    public string? Tense { get; init; }
    
    /// <summary>Verb voice (e.g., "Active", "Middle", "Passive")</summary>
    public string? Voice { get; init; }
    
    /// <summary>Verb mood (e.g., "Indicative", "Subjunctive", "Participle")</summary>
    public string? Mood { get; init; }
    
    /// <summary>Grammatical case (e.g., "Nominative", "Genitive", "Accusative")</summary>
    public string? Case { get; init; }
    
    /// <summary>Grammatical number (e.g., "Singular", "Plural")</summary>
    public string? Number { get; init; }
    
    /// <summary>Grammatical gender (e.g., "Masculine", "Feminine", "Neuter")</summary>
    public string? Gender { get; init; }
    
    /// <summary>Person for verbs and pronouns (e.g., "First", "Second", "Third")</summary>
    public string? Person { get; init; }
    
    /// <summary>Optional descriptor suffix (e.g., "Person", "Title", "Comparative")</summary>
    public string? Suffix { get; init; }
}
