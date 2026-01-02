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

    // Finite moods only (Indicative, Subjunctive, Imperative, Optative)
    private static readonly Dictionary<char, string> FiniteMoodMap = new()
    {
        ['I'] = "Indicative",
        ['S'] = "Subjunctive",
        ['M'] = "Imperative",
        ['O'] = "Optative"
    };

    private static readonly Dictionary<string, string> FlagMap = new()
    {
        ["C"] = "Comparative",
        ["I"] = "Interrogative",
        ["K"] = "Krasis",
        ["L"] = "Location",
        ["LG"] = "LocationGentilic",
        ["LI"] = "LetterIndeclinable",
        ["N"] = "Negative",
        ["NUI"] = "IndeclinableNumber",
        ["P"] = "ProperName",
        ["PG"] = "PersonGentilic",
        ["S"] = "Superlative",
        ["T"] = "Title"
    };

    // Voice flags for deponent verbs
    private static readonly HashSet<char> DeponentVoices = new() { 'D', 'N', 'O' };

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

        // Handle Personal Pronouns (P-...) - special format
        if (code.StartsWith("P-"))
            return TryParsePersonalPronoun(code, out result);

        // Handle Nominals (N-, A-, T-, Q-, etc.)
        return TryParseNominal(code, out result);
    }

    /// <summary>
    /// Parses simple/indeclinable types like CONJ, PREP, INJ
    /// </summary>
    private static bool TryParseSimpleType(string code, out MorphologyInfo? result)
    {
        result = null;
        var basePart = code.Split('-')[0];

        if (!SimpleTypes.Contains(basePart))
            return false;

        var pos = PartOfSpeechMap.GetValueOrDefault(basePart);
        var flags = new List<string>();

        // Check for suffix (e.g., CONJ-T, PREP-G)
        if (code.Contains('-'))
        {
            var suffixPart = code.Split('-')[1];
            var flag = FlagMap.GetValueOrDefault(suffixPart);
            if (flag != null)
                flags.Add(flag);
        }

        result = new MorphologyInfo { Pos = pos, Flags = flags };
        return true;
    }

    /// <summary>
    /// Parses adverbs (ADV, ADV-C, ADV-I, etc.)
    /// </summary>
    private static bool TryParseAdverb(string code, out MorphologyInfo? result)
    {
        var flags = new List<string>();

        if (code.Contains('-'))
        {
            var suffixPart = code.Split('-')[1];
            var flag = FlagMap.GetValueOrDefault(suffixPart);
            if (flag != null)
                flags.Add(flag);
        }

        result = new MorphologyInfo { Pos = "Adverb", Flags = flags };
        return true;
    }

    /// <summary>
    /// Parses verb codes (V-TVM-PN or V-TVN or V-TVP-CNG)
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
        var voiceChar = modifiers[voiceIndex];
        var voice = VoiceMap.GetValueOrDefault(voiceChar);
        var moodChar = modifiers[voiceIndex + 1];

        // Build flags list
        var flags = new List<string>();
        if (DeponentVoices.Contains(voiceChar))
            flags.Add("Deponent");

        // Determine verbForm and mood based on moodChar
        string verbForm;
        string? mood;

        if (moodChar == 'N')
        {
            verbForm = "Infinitive";
            mood = null;
        }
        else if (moodChar == 'P')
        {
            verbForm = "Participle";
            mood = null;
        }
        else
        {
            verbForm = "Finite";
            mood = FiniteMoodMap.GetValueOrDefault(moodChar);
        }

        result = new MorphologyInfo
        {
            Pos = "Verb",
            Tense = tense,
            Voice = voice,
            VerbForm = verbForm,
            Mood = mood,
            Flags = flags
        };

        // Parse additional modifiers based on verb form
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
    /// Parses personal pronouns (P-1NS, P-2GS, P-3APM)
    /// Format: P-{Person}{Case}{Number} or P-{Case}{Number}{Gender} for 3rd person
    /// </summary>
    private static bool TryParsePersonalPronoun(string code, out MorphologyInfo? result)
    {
        result = null;
        var parts = code.Split('-');

        if (parts.Length < 2)
            return false;

        var modifiers = parts[1];
        if (modifiers.Length < 2)
            return false;

        string? person = null;
        string? grammaticalCase = null;
        string? number = null;
        string? gender = null;
        var flags = new List<string>();

        // Check if first char is person (1, 2, 3)
        if (PersonMap.ContainsKey(modifiers[0]))
        {
            // Format: P-{Person}{Case}{Number} (1st/2nd person)
            person = PersonMap.GetValueOrDefault(modifiers[0]);
            if (modifiers.Length > 1)
                grammaticalCase = CaseMap.GetValueOrDefault(modifiers[1]);
            if (modifiers.Length > 2)
                number = NumberMap.GetValueOrDefault(modifiers[2]);
        }
        else
        {
            // Format: P-{Case}{Number}{Gender} (3rd person implied)
            person = "Third";
            grammaticalCase = CaseMap.GetValueOrDefault(modifiers[0]);
            if (modifiers.Length > 1)
                number = NumberMap.GetValueOrDefault(modifiers[1]);
            if (modifiers.Length > 2)
                gender = GenderMap.GetValueOrDefault(modifiers[2]);
        }

        // Check for suffix flags
        if (parts.Length >= 3)
        {
            var flag = FlagMap.GetValueOrDefault(parts[2]);
            if (flag != null)
                flags.Add(flag);
        }

        result = new MorphologyInfo
        {
            Pos = "PersonalPronoun",
            Person = person,
            Case = grammaticalCase,
            Number = number,
            Gender = gender,
            Flags = flags
        };

        return true;
    }

    /// <summary>
    /// Parses nominal types (Noun, Adjective, Article, other Pronouns)
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

        var modifiers = parts[1];
        var flags = new List<string>();

        // Parse case/number/gender
        string? grammaticalCase = null;
        string? number = null;
        string? gender = null;

        if (modifiers.Length > 0)
            grammaticalCase = CaseMap.GetValueOrDefault(modifiers[0]);
        if (modifiers.Length > 1)
            number = NumberMap.GetValueOrDefault(modifiers[1]);
        if (modifiers.Length > 2)
            gender = GenderMap.GetValueOrDefault(modifiers[2]);

        // Check for suffix flags (e.g., -P for ProperName, -T for Title)
        if (parts.Length >= 3)
        {
            var flag = FlagMap.GetValueOrDefault(parts[2]);
            if (flag != null)
                flags.Add(flag);
        }

        result = new MorphologyInfo
        {
            Pos = pos,
            Case = grammaticalCase,
            Number = number,
            Gender = gender,
            Flags = flags
        };

        return true;
    }
}
