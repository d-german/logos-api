using System.Text.RegularExpressions;

namespace LogosAPI.Services;

/// <summary>
/// Normalizes Bible verse references to canonical format (Book.Chapter.Verse)
/// Single Responsibility: Parse and normalize verse reference strings
/// </summary>
public sealed partial class VerseReferenceNormalizer : IVerseReferenceNormalizer
{
    private static readonly Dictionary<string, string> BookAliases = CreateBookAliases();

    /// <summary>
    /// Regex pattern to parse verse references
    /// Groups: 1=Number prefix (optional), 2=Book name, 3=Chapter, 4=Verse
    /// Cyclomatic Complexity: 1
    /// </summary>
    [GeneratedRegex(@"^\s*(?:(\d|I{1,3}|First|Second|Third)\s*)?([A-Za-z]+)[\s\.]*(\d+)[\s:\.\-]+(\d+)\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex VerseReferencePattern();

    /// <inheritdoc />
    public string Normalize(string input)
    {
        if (!TryNormalize(input, out var normalized))
        {
            throw new ArgumentException($"Invalid verse reference: '{input}'", nameof(input));
        }

        return normalized!;
    }

    /// <inheritdoc />
    public bool TryNormalize(string input, out string? normalized)
    {
        normalized = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var match = VerseReferencePattern().Match(input);
        if (!match.Success)
            return false;

        return TryBuildNormalizedReference(match, out normalized);
    }

    /// <inheritdoc />
    public bool IsValid(string input)
    {
        return TryNormalize(input, out _);
    }

    /// <summary>
    /// Builds normalized reference from regex match
    /// Cyclomatic Complexity: 3
    /// </summary>
    private static bool TryBuildNormalizedReference(Match match, out string? normalized)
    {
        normalized = null;

        var numberPrefix = NormalizeNumberPrefix(match.Groups[1].Value);
        var bookName = match.Groups[2].Value;
        var chapter = match.Groups[3].Value.TrimStart('0');
        var verse = match.Groups[4].Value.TrimStart('0');

        if (string.IsNullOrEmpty(chapter)) chapter = "0";
        if (string.IsNullOrEmpty(verse)) verse = "0";

        var canonicalBook = GetCanonicalBookName(numberPrefix, bookName);
        if (canonicalBook is null)
            return false;

        normalized = FormatReference(canonicalBook, chapter, verse);
        return true;
    }

    /// <summary>
    /// Normalizes number prefix (I, II, III, First, Second, Third → 1, 2, 3)
    /// Cyclomatic Complexity: 4
    /// </summary>
    private static string NormalizeNumberPrefix(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return string.Empty;

        return prefix.ToUpperInvariant() switch
        {
            "I" or "FIRST" => "1",
            "II" or "SECOND" => "2",
            "III" or "THIRD" => "3",
            _ => prefix
        };
    }

    /// <summary>
    /// Gets canonical book name from alias
    /// Cyclomatic Complexity: 2
    /// </summary>
    private static string? GetCanonicalBookName(string numberPrefix, string bookName)
    {
        var lookupKey = (numberPrefix + bookName).ToLowerInvariant();
        return BookAliases.GetValueOrDefault(lookupKey);
    }

    /// <summary>
    /// Formats the normalized reference
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static string FormatReference(string book, string chapter, string verse)
    {
        return $"{book}.{chapter}.{verse}";
    }

    /// <summary>
    /// Creates the book alias dictionary
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static Dictionary<string, string> CreateBookAliases()
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Matthew
            { "matthew", "Matt" },
            { "matt", "Matt" },
            { "mat", "Matt" },
            { "mt", "Matt" },

            // Mark
            { "mark", "Mark" },
            { "mrk", "Mark" },
            { "mk", "Mark" },
            { "mr", "Mark" },

            // Luke
            { "luke", "Luke" },
            { "luk", "Luke" },
            { "lk", "Luke" },

            // John
            { "john", "John" },
            { "jhn", "John" },
            { "jn", "John" },

            // Acts
            { "acts", "Acts" },
            { "act", "Acts" },
            { "ac", "Acts" },

            // Romans
            { "romans", "Rom" },
            { "rom", "Rom" },
            { "rm", "Rom" },
            { "ro", "Rom" },

            // 1 Corinthians
            { "1corinthians", "1Cor" },
            { "1cor", "1Cor" },
            { "1co", "1Cor" },

            // 2 Corinthians
            { "2corinthians", "2Cor" },
            { "2cor", "2Cor" },
            { "2co", "2Cor" },

            // Galatians
            { "galatians", "Gal" },
            { "gal", "Gal" },
            { "ga", "Gal" },

            // Ephesians
            { "ephesians", "Eph" },
            { "eph", "Eph" },
            { "ep", "Eph" },

            // Philippians
            { "philippians", "Phil" },
            { "phil", "Phil" },
            { "php", "Phil" },
            { "pp", "Phil" },

            // Colossians
            { "colossians", "Col" },
            { "col", "Col" },

            // 1 Thessalonians
            { "1thessalonians", "1Thess" },
            { "1thess", "1Thess" },
            { "1thes", "1Thess" },
            { "1th", "1Thess" },

            // 2 Thessalonians
            { "2thessalonians", "2Thess" },
            { "2thess", "2Thess" },
            { "2thes", "2Thess" },
            { "2th", "2Thess" },

            // 1 Timothy
            { "1timothy", "1Tim" },
            { "1tim", "1Tim" },
            { "1ti", "1Tim" },

            // 2 Timothy
            { "2timothy", "2Tim" },
            { "2tim", "2Tim" },
            { "2ti", "2Tim" },

            // Titus
            { "titus", "Titus" },
            { "tit", "Titus" },
            { "ti", "Titus" },

            // Philemon
            { "philemon", "Phlm" },
            { "phlm", "Phlm" },
            { "phm", "Phlm" },
            { "pm", "Phlm" },

            // Hebrews
            { "hebrews", "Heb" },
            { "heb", "Heb" },
            { "he", "Heb" },

            // James
            { "james", "Jas" },
            { "jas", "Jas" },
            { "jm", "Jas" },
            { "jam", "Jas" },

            // 1 Peter
            { "1peter", "1Pet" },
            { "1pet", "1Pet" },
            { "1pe", "1Pet" },
            { "1pt", "1Pet" },

            // 2 Peter
            { "2peter", "2Pet" },
            { "2pet", "2Pet" },
            { "2pe", "2Pet" },
            { "2pt", "2Pet" },

            // 1 John
            { "1john", "1John" },
            { "1jhn", "1John" },
            { "1jn", "1John" },

            // 2 John
            { "2john", "2John" },
            { "2jhn", "2John" },
            { "2jn", "2John" },

            // 3 John
            { "3john", "3John" },
            { "3jhn", "3John" },
            { "3jn", "3John" },

            // Jude
            { "jude", "Jude" },
            { "jud", "Jude" },
            { "jd", "Jude" },

            // Revelation
            { "revelation", "Rev" },
            { "revelations", "Rev" },
            { "rev", "Rev" },
            { "re", "Rev" },
            { "apocalypse", "Rev" },
        };
    }
}
