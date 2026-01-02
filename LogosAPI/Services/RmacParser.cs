using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Parses Robinson's Morphological Analysis Codes (RMAC) into structured components.
/// Single Responsibility: Parse RMAC code strings into MorphologyInfo objects.
/// </summary>
public sealed class RmacParser : IRmacParser
{
    #region Lookup Dictionaries

    private static readonly Dictionary<string, string> PartOfSpeechMap = new()
    {
        ["A"] = "Adjective",
        ["ADV"] = "Adverb",
        ["ARAM"] = "Aramaic",
        ["C"] = "ReciprocalPronoun",
        ["CONJ"] = "Conjunction",
        ["D"] = "DemonstrativePronoun",
        ["F"] = "ReflexivePronoun",
        ["HEB"] = "Hebrew",
        ["I"] = "InterrogativePronoun",
        ["INJ"] = "Interjection",
        ["K"] = "CorrelativePronoun",
        ["N"] = "Noun",
        ["P"] = "PersonalPronoun",
        ["PREP"] = "Preposition",
        ["PRT"] = "Particle",
        ["Q"] = "CorrelativeAdjective",
        ["R"] = "RelativePronoun",
        ["S"] = "PossessivePronoun",
        ["T"] = "Article",
        ["V"] = "Verb",
        ["X"] = "IndefinitePronoun"
    };

    private static readonly Dictionary<char, string> CaseMap = new()
    {
        ['A'] = "Accusative",
        ['D'] = "Dative",
        ['G'] = "Genitive",
        ['N'] = "Nominative",
        ['V'] = "Vocative"
    };

    private static readonly Dictionary<char, string> NumberMap = new()
    {
        ['S'] = "Singular",
        ['P'] = "Plural"
    };

    private static readonly Dictionary<char, string> GenderMap = new()
    {
        ['M'] = "Masculine",
        ['F'] = "Feminine",
        ['N'] = "Neuter"
    };

    private static readonly Dictionary<char, string> PersonMap = new()
    {
        ['1'] = "First",
        ['2'] = "Second",
        ['3'] = "Third"
    };

    private static readonly Dictionary<string, string> TenseMap = new()
    {
        ["P"] = "Present",
        ["I"] = "Imperfect",
        ["F"] = "Future",
        ["A"] = "Aorist",
        ["R"] = "Perfect",
        ["L"] = "Pluperfect",
        ["2P"] = "SecondPresent",
        ["2I"] = "SecondImperfect",
        ["2F"] = "SecondFuture",
        ["2A"] = "SecondAorist",
        ["2R"] = "SecondPerfect",
        ["2L"] = "SecondPluperfect"
    };

    private static readonly Dictionary<char, string> VoiceMap = new()
    {
        ['A'] = "Active",
        ['M'] = "Middle",
        ['P'] = "Passive",
        ['E'] = "MiddleOrPassive",
        ['D'] = "Deponent",
        ['N'] = "MiddleOrPassiveDeponent",
        ['O'] = "PassiveDeponent"
    };

    private static readonly Dictionary<char, string> MoodMap = new()
    {
        ['I'] = "Indicative",
        ['S'] = "Subjunctive",
        ['M'] = "Imperative",
        ['O'] = "Optative",
        ['N'] = "Infinitive",
        ['P'] = "Participle"
    };

    private static readonly Dictionary<string, string> SuffixMap = new()
    {
        ["C"] = "Comparative",
        ["I"] = "Interrogative",
        ["K"] = "Krasis",
        ["L"] = "Location",
        ["LG"] = "LocationGentilic",
        ["LI"] = "LetterIndeclinable",
        ["N"] = "Negative",
        ["NUI"] = "IndeclinableNumber",
        ["P"] = "Person",
        ["PG"] = "PersonGentilic",
        ["S"] = "Superlative",
        ["T"] = "Title",
        ["A"] = "Accusative",
        ["D"] = "Dative",
        ["G"] = "Genitive"
    };

    private static readonly HashSet<string> SimpleTypes = new()
    {
        "CONJ", "INJ", "ARAM", "HEB", "PRT", "PREP"
    };

    #endregion

    /// <inheritdoc />
    public MorphologyInfo? Parse(string? rmacCode)
    {
        TryParse(rmacCode, out var result);
        return result;
    }

    /// <inheritdoc />
    public bool TryParse(string? rmacCode, out MorphologyInfo? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(rmacCode))
            return false;

        var code = rmacCode.Trim().ToUpperInvariant();

        // Handle simple/indeclinable types (CONJ, PREP, etc.)
        if (TryParseSimpleType(code, out result))
            return true;

        // Handle Adverbs (ADV, ADV-C, ADV-N, etc.)
        if (code.StartsWith("ADV"))
            return TryParseAdverb(code, out result);

        // Handle Verbs (V-...)
        if (code.StartsWith("V-"))
            return TryParseVerb(code, out result);

        // Handle Nominals (N-, A-, T-, Q-, etc.)
        return TryParseNominal(code, out result);
    }

    /// <summary>
    /// Parses simple/indeclinable types like CONJ, PREP, INJ
    /// Cyclomatic Complexity: 3
    /// </summary>
    private static bool TryParseSimpleType(string code, out MorphologyInfo? result)
    {
        result = null;
        var basePart = code.Split('-')[0];

        if (!SimpleTypes.Contains(basePart))
            return false;

        var pos = PartOfSpeechMap.GetValueOrDefault(basePart);
        string? suffix = null;

        // Check for suffix (e.g., CONJ-T, PREP-G)
        if (code.Contains('-'))
        {
            var suffixPart = code.Split('-')[1];
            suffix = SuffixMap.GetValueOrDefault(suffixPart);
        }

        result = new MorphologyInfo { Pos = pos, Suffix = suffix };
        return true;
    }

    /// <summary>
    /// Parses adverbs (ADV, ADV-C, ADV-I, etc.)
    /// Cyclomatic Complexity: 2
    /// </summary>
    private static bool TryParseAdverb(string code, out MorphologyInfo? result)
    {
        string? suffix = null;

        if (code.Contains('-'))
        {
            var suffixPart = code.Split('-')[1];
            suffix = SuffixMap.GetValueOrDefault(suffixPart);
        }

        result = new MorphologyInfo { Pos = "Adverb", Suffix = suffix };
        return true;
    }

    /// <summary>
    /// Parses verb codes (V-TVM-PN or V-TVN or V-TVP-CNG)
    /// Cyclomatic Complexity: 5
    /// </summary>
    private static bool TryParseVerb(string code, out MorphologyInfo? result)
    {
        result = null;
        var parts = code.Split('-');

        if (parts.Length < 2)
            return false;

        var modifiers = parts[1];
        if (modifiers.Length < 3)
            return false;

        // Extract tense (may be 2-char for secondary forms)
        string tenseKey;
        int voiceIndex;

        if (modifiers[0] == '2' && modifiers.Length >= 4)
        {
            tenseKey = modifiers[..2];
            voiceIndex = 2;
        }
        else
        {
            tenseKey = modifiers[0].ToString();
            voiceIndex = 1;
        }

        var tense = TenseMap.GetValueOrDefault(tenseKey);
        var voice = VoiceMap.GetValueOrDefault(modifiers[voiceIndex]);
        var mood = MoodMap.GetValueOrDefault(modifiers[voiceIndex + 1]);

        result = new MorphologyInfo
        {
            Pos = "Verb",
            Tense = tense,
            Voice = voice,
            Mood = mood
        };

        // Check mood type to determine remaining parsing
        var moodChar = modifiers[voiceIndex + 1];

        if (moodChar == 'N') // Infinitive - no more components
        {
            return true;
        }

        if (moodChar == 'P' && parts.Length >= 3) // Participle - has CNG
        {
            result = ParseParticipleModifiers(result, parts[2]);
            return true;
        }

        if (parts.Length >= 3) // Finite verb - has PN
        {
            result = ParseFiniteVerbModifiers(result, parts[2]);
        }

        return true;
    }

    /// <summary>
    /// Parses participle modifiers (Case-Number-Gender)
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static MorphologyInfo ParseParticipleModifiers(MorphologyInfo info, string cng)
    {
        return info with
        {
            Case = cng.Length > 0 ? CaseMap.GetValueOrDefault(cng[0]) : null,
            Number = cng.Length > 1 ? NumberMap.GetValueOrDefault(cng[1]) : null,
            Gender = cng.Length > 2 ? GenderMap.GetValueOrDefault(cng[2]) : null
        };
    }

    /// <summary>
    /// Parses finite verb modifiers (Person-Number)
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static MorphologyInfo ParseFiniteVerbModifiers(MorphologyInfo info, string pn)
    {
        return info with
        {
            Person = pn.Length > 0 ? PersonMap.GetValueOrDefault(pn[0]) : null,
            Number = pn.Length > 1 ? NumberMap.GetValueOrDefault(pn[1]) : null
        };
    }

    /// <summary>
    /// Parses nominal types (Noun, Adjective, Article, Pronouns)
    /// Cyclomatic Complexity: 4
    /// </summary>
    private static bool TryParseNominal(string code, out MorphologyInfo? result)
    {
        result = null;
        var parts = code.Split('-');

        if (parts.Length < 2)
            return false;

        var typePart = parts[0];
        var pos = PartOfSpeechMap.GetValueOrDefault(typePart);

        if (pos is null)
            return false;

        var cng = parts[1];

        result = new MorphologyInfo
        {
            Pos = pos,
            Case = cng.Length > 0 ? CaseMap.GetValueOrDefault(cng[0]) : null,
            Number = cng.Length > 1 ? NumberMap.GetValueOrDefault(cng[1]) : null,
            Gender = cng.Length > 2 ? GenderMap.GetValueOrDefault(cng[2]) : null
        };

        // Check for suffix (e.g., -P for Person, -T for Title)
        if (parts.Length >= 3)
        {
            result = result with { Suffix = SuffixMap.GetValueOrDefault(parts[2]) };
        }

        return true;
    }
}
