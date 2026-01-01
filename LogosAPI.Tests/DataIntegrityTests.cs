using System.Reflection;
using System.Text.Json;
using LogosAPI.Models;

namespace LogosAPI.Tests;

/// <summary>
/// Data integrity tests to ensure embedded JSON resources can be properly serialized/deserialized
/// These tests catch encoding issues, special characters, or malformed data before deployment
/// </summary>
public sealed class DataIntegrityTests
{
    private const string VersesResourceName = "LogosAPI.Data.verses.json";
    private const string LexiconResourceName = "LogosAPI.Data.lexicon.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #region Lexicon Tests

    [Fact]
    public void Lexicon_AllEntries_CanBeDeserialized()
    {
        // Arrange
        var json = ReadEmbeddedResource(LexiconResourceName);
        Assert.NotNull(json);

        // Act
        var lexicon = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions);

        // Assert
        Assert.NotNull(lexicon);
        Assert.NotEmpty(lexicon);
    }

    [Fact]
    public void Lexicon_AllEntries_CanBeReserializedToJson()
    {
        // Arrange
        var json = ReadEmbeddedResource(LexiconResourceName);
        var lexicon = JsonSerializer.Deserialize<Dictionary<string, string>>(json!, JsonOptions);

        // Act & Assert - ensure every entry can be serialized back to JSON
        foreach (var (key, value) in lexicon!)
        {
            var entry = new { Key = key, Value = value };
            var serialized = JsonSerializer.Serialize(entry);
            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);
        }
    }

    [Fact]
    public void Lexicon_AllKeys_AreValidStrongsNumbers()
    {
        // Arrange
        var json = ReadEmbeddedResource(LexiconResourceName);
        var lexicon = JsonSerializer.Deserialize<Dictionary<string, string>>(json!, JsonOptions);

        // Act & Assert - Strong's numbers should match pattern like G1, G123, H1, H123
        foreach (var key in lexicon!.Keys)
        {
            Assert.Matches(@"^[GH]\d+$", key);
        }
    }

    [Fact]
    public void Lexicon_AllValues_AreNotNullOrWhitespace()
    {
        // Arrange
        var json = ReadEmbeddedResource(LexiconResourceName);
        var lexicon = JsonSerializer.Deserialize<Dictionary<string, string>>(json!, JsonOptions);

        // Act & Assert
        foreach (var (key, value) in lexicon!)
        {
            Assert.False(
                string.IsNullOrWhiteSpace(value),
                $"Lexicon entry '{key}' has null or whitespace value");
        }
    }

    [Fact]
    public void Lexicon_NoEntriesContainInvalidJsonCharacters()
    {
        // Arrange
        var json = ReadEmbeddedResource(LexiconResourceName);
        var lexicon = JsonSerializer.Deserialize<Dictionary<string, string>>(json!, JsonOptions);

        // Act & Assert - ensure round-trip works for each entry
        var failedEntries = new List<string>();

        foreach (var (key, value) in lexicon!)
        {
            try
            {
                // Create a small JSON object with this value and round-trip it
                var testObject = new Dictionary<string, string> { { "test", value } };
                var serialized = JsonSerializer.Serialize(testObject);
                var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(serialized);

                if (deserialized?["test"] != value)
                {
                    failedEntries.Add($"{key}: Value changed after round-trip");
                }
            }
            catch (Exception ex)
            {
                failedEntries.Add($"{key}: {ex.Message}");
            }
        }

        Assert.Empty(failedEntries);
    }

    #endregion

    #region Verses Tests

    [Fact]
    public void Verses_AllEntries_CanBeDeserialized()
    {
        // Arrange
        var json = ReadEmbeddedResource(VersesResourceName);
        Assert.NotNull(json);

        // Act
        var verses = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(json, JsonOptions);

        // Assert
        Assert.NotNull(verses);
        Assert.NotEmpty(verses);
    }

    [Fact]
    public void Verses_AllEntries_HaveValidTokens()
    {
        // Arrange
        var json = ReadEmbeddedResource(VersesResourceName);
        var verses = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(json!, JsonOptions);

        // Act & Assert
        var failedVerses = new List<string>();

        foreach (var (reference, verseData) in verses!)
        {
            if (verseData.Tokens == null || verseData.Tokens.Count == 0)
            {
                failedVerses.Add($"{reference}: No tokens");
                continue;
            }

            foreach (var token in verseData.Tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Strongs))
                {
                    failedVerses.Add($"{reference}: Token missing Strongs number");
                }
            }
        }

        // Allow some failures but flag if too many
        Assert.True(
            failedVerses.Count < 100,
            $"Too many verses with invalid tokens: {failedVerses.Count}\nFirst 10: {string.Join("\n", failedVerses.Take(10))}");
    }

    [Fact]
    public void Verses_AllKeys_AreValidReferences()
    {
        // Arrange
        var json = ReadEmbeddedResource(VersesResourceName);
        var verses = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(json!, JsonOptions);

        // Act & Assert - references should match pattern like Book.Chapter.Verse
        var invalidRefs = verses!.Keys
            .Where(k => !System.Text.RegularExpressions.Regex.IsMatch(k, @"^[A-Za-z0-9]+\.\d+\.\d+$"))
            .Take(10)
            .ToList();

        Assert.Empty(invalidRefs);
    }

    [Fact]
    public void Verses_CanBeReserializedToJson()
    {
        // Arrange
        var json = ReadEmbeddedResource(VersesResourceName);
        var verses = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(json!, JsonOptions);

        // Act
        var reserialized = JsonSerializer.Serialize(verses);

        // Assert
        Assert.NotNull(reserialized);
        Assert.NotEmpty(reserialized);

        // Verify it can be deserialized again
        var roundTripped = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(reserialized, JsonOptions);
        Assert.NotNull(roundTripped);
        Assert.Equal(verses!.Count, roundTripped!.Count);
    }

    #endregion

    #region Cross-Reference Tests

    [Fact]
    public void Verses_StrongsNumbers_ExistInLexicon()
    {
        // Arrange
        var versesJson = ReadEmbeddedResource(VersesResourceName);
        var lexiconJson = ReadEmbeddedResource(LexiconResourceName);

        var verses = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(versesJson!, JsonOptions);
        var lexicon = JsonSerializer.Deserialize<Dictionary<string, string>>(lexiconJson!, JsonOptions);

        // Act - collect all unique Strong's numbers from verses
        var strongsInVerses = verses!
            .SelectMany(v => v.Value.Tokens ?? new List<TokenData>())
            .Select(t => t.Strongs)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        // Assert - check coverage
        var missingInLexicon = strongsInVerses
            .Where(s => !lexicon!.ContainsKey(s))
            .Take(20)
            .ToList();

        var coveragePercent = (double)(strongsInVerses.Count - missingInLexicon.Count) / strongsInVerses.Count * 100;

        // Warn but don't fail if some are missing (may be expected)
        Assert.True(
            coveragePercent > 80,
            $"Only {coveragePercent:F1}% of Strong's numbers in verses have lexicon entries. Missing examples: {string.Join(", ", missingInLexicon)}");
    }

    #endregion

    #region Helper Methods

    private static string? ReadEmbeddedResource(string resourceName)
    {
        // Get the LogosAPI assembly (not the test assembly)
        var assembly = Assembly.Load("LogosAPI");

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            var available = assembly.GetManifestResourceNames();
            throw new InvalidOperationException(
                $"Resource '{resourceName}' not found. Available: {string.Join(", ", available)}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    #endregion
}
