using System.Text.RegularExpressions;

namespace LogosAPI.Services;

/// <summary>
/// Normalizes Strong's Concordance numbers to canonical format (e.g., G25, H1234)
/// Single Responsibility: Parse and normalize Strong's number strings
/// </summary>
public sealed partial class StrongsNumberNormalizer : IStrongsNumberNormalizer
{
    /// <summary>
    /// Regex pattern to parse Strong's numbers
    /// Groups: 1=Prefix (G or H), 2=Number (with leading zeros stripped by regex)
    /// Cyclomatic Complexity: 1
    /// </summary>
    [GeneratedRegex(@"^\s*([GgHh])\s*0*(\d+)\s*$", RegexOptions.Compiled)]
    private static partial Regex StrongsPattern();

    /// <inheritdoc />
    public string Normalize(string input)
    {
        if (!TryNormalize(input, out var normalized))
        {
            throw new ArgumentException($"Invalid Strong's number: '{input}'", nameof(input));
        }

        return normalized!;
    }

    /// <inheritdoc />
    public bool TryNormalize(string input, out string? normalized)
    {
        normalized = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var match = StrongsPattern().Match(input);
        if (!match.Success)
            return false;

        return TryBuildNormalizedStrongs(match, out normalized);
    }

    /// <inheritdoc />
    public bool IsValid(string input)
    {
        return TryNormalize(input, out _);
    }

    /// <summary>
    /// Builds normalized Strong's number from regex match
    /// Cyclomatic Complexity: 2
    /// </summary>
    private static bool TryBuildNormalizedStrongs(Match match, out string? normalized)
    {
        var prefix = match.Groups[1].Value.ToUpperInvariant();
        var number = match.Groups[2].Value;

        // Handle edge case where number is empty after trimming (e.g., "G0" -> "G0")
        if (string.IsNullOrEmpty(number))
            number = "0";

        normalized = FormatStrongsNumber(prefix, number);
        return true;
    }

    /// <summary>
    /// Formats the normalized Strong's number
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static string FormatStrongsNumber(string prefix, string number)
    {
        return $"{prefix}{number}";
    }
}
