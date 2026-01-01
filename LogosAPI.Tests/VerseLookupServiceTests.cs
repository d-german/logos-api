using System.Collections.Concurrent;
using LogosAPI.Models;
using LogosAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for VerseLookupService
/// </summary>
public sealed class VerseLookupServiceTests
{
    private readonly Mock<IBibleDataService> _mockBibleDataService;
    private readonly Mock<IVerseReferenceNormalizer> _mockNormalizer;
    private readonly Mock<ILogger<VerseLookupService>> _mockLogger;
    private readonly VerseLookupService _service;

    public VerseLookupServiceTests()
    {
        _mockBibleDataService = new Mock<IBibleDataService>();
        _mockNormalizer = new Mock<IVerseReferenceNormalizer>();
        _mockLogger = new Mock<ILogger<VerseLookupService>>();

        // Setup default empty dictionaries
        _mockBibleDataService.Setup(x => x.Verses)
            .Returns(new ConcurrentDictionary<string, VerseData>());
        _mockBibleDataService.Setup(x => x.Lexicon)
            .Returns(new ConcurrentDictionary<string, string>());

        _service = new VerseLookupService(
            _mockBibleDataService.Object,
            _mockNormalizer.Object,
            _mockLogger.Object);
    }

    #region Happy Path Tests

    [Fact]
    public void LookupVerses_ValidReference_ReturnsVerseWithTokens()
    {
        // Arrange
        var reference = "Matt.1.1";
        var tokens = new List<TokenData>
        {
            new() { Gloss = "book", Greek = "Βίβλος", Translit = "biblos", Strongs = "G976", Rmac = "N-NSF", RmacDesc = "Noun" }
        };
        var verseData = new VerseData { Tokens = tokens };

        SetupNormalizer("Matthew 1:1", reference);
        SetupVerse(reference, verseData);
        SetupLexicon("G976", "A book, a written document");

        // Act
        var result = _service.LookupVerses(new[] { "Matthew 1:1" });

        // Assert
        Assert.Single(result.Verses);
        Assert.Empty(result.NotFound);
        Assert.Equal(reference, result.Verses[0].Reference);
        Assert.Single(result.Verses[0].Tokens);
        Assert.Equal("A book, a written document", result.Verses[0].Tokens[0].LexiconEntry);
    }

    [Fact]
    public void LookupVerses_MultipleVerses_ReturnsAll()
    {
        // Arrange
        SetupNormalizer("Matt 1:1", "Matt.1.1");
        SetupNormalizer("John 3:16", "John.3.16");
        SetupVerses(
            ("Matt.1.1", CreateVerseData("word1", "G1")),
            ("John.3.16", CreateVerseData("word2", "G2"))
        );

        // Act
        var result = _service.LookupVerses(new[] { "Matt 1:1", "John 3:16" });

        // Assert
        Assert.Equal(2, result.Verses.Count);
        Assert.Empty(result.NotFound);
    }

    #endregion

    #region Normalization Tests

    [Fact]
    public void LookupVerses_InvalidReference_AddsToNotFound()
    {
        // Arrange
        SetupNormalizerFails("InvalidRef");

        // Act
        var result = _service.LookupVerses(new[] { "InvalidRef" });

        // Assert
        Assert.Empty(result.Verses);
        Assert.Single(result.NotFound);
        Assert.Equal("InvalidRef", result.NotFound[0]);
    }

    [Fact]
    public void LookupVerses_MixedValidInvalid_HandlesBoth()
    {
        // Arrange
        SetupNormalizer("Matt 1:1", "Matt.1.1");
        SetupNormalizerFails("BadRef");
        SetupVerse("Matt.1.1", CreateVerseData("word", "G1"));

        // Act
        var result = _service.LookupVerses(new[] { "Matt 1:1", "BadRef" });

        // Assert
        Assert.Single(result.Verses);
        Assert.Single(result.NotFound);
        Assert.Equal("BadRef", result.NotFound[0]);
    }

    #endregion

    #region Verse Lookup Tests

    [Fact]
    public void LookupVerses_VerseNotInDictionary_AddsToNotFound()
    {
        // Arrange
        SetupNormalizer("Matt 1:1", "Matt.1.1");
        // Don't add verse to dictionary

        // Act
        var result = _service.LookupVerses(new[] { "Matt 1:1" });

        // Assert
        Assert.Empty(result.Verses);
        Assert.Single(result.NotFound);
        Assert.Equal("Matt.1.1", result.NotFound[0]);
    }

    #endregion

    #region Lexicon Enrichment Tests

    [Fact]
    public void LookupVerses_UnknownStrongs_LexiconEntryIsNull()
    {
        // Arrange
        SetupNormalizer("Matt 1:1", "Matt.1.1");
        SetupVerse("Matt.1.1", CreateVerseData("word", "G99999"));
        // Don't add lexicon entry for G99999

        // Act
        var result = _service.LookupVerses(new[] { "Matt 1:1" });

        // Assert
        Assert.Single(result.Verses);
        Assert.Null(result.Verses[0].Tokens[0].LexiconEntry);
    }

    [Fact]
    public void LookupVerses_KnownStrongs_LexiconEntryPopulated()
    {
        // Arrange
        SetupNormalizer("Matt 1:1", "Matt.1.1");
        SetupVerse("Matt.1.1", CreateVerseData("word", "G976"));
        SetupLexicon("G976", "Test definition");

        // Act
        var result = _service.LookupVerses(new[] { "Matt 1:1" });

        // Assert
        Assert.Equal("Test definition", result.Verses[0].Tokens[0].LexiconEntry);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void LookupVerses_EmptyInput_ReturnsEmptyResult()
    {
        // Act
        var result = _service.LookupVerses(Array.Empty<string>());

        // Assert
        Assert.Empty(result.Verses);
        Assert.Empty(result.NotFound);
    }

    [Fact]
    public void LookupVerses_MultipleTokensInVerse_AllEnrichedWithLexicon()
    {
        // Arrange
        var tokens = new List<TokenData>
        {
            new() { Gloss = "word1", Greek = "Α", Translit = "a", Strongs = "G1", Rmac = "N", RmacDesc = "Noun" },
            new() { Gloss = "word2", Greek = "Β", Translit = "b", Strongs = "G2", Rmac = "V", RmacDesc = "Verb" },
            new() { Gloss = "word3", Greek = "Γ", Translit = "c", Strongs = "G3", Rmac = "A", RmacDesc = "Adj" }
        };
        var verseData = new VerseData { Tokens = tokens };

        SetupNormalizer("Matt 1:1", "Matt.1.1");
        SetupVerse("Matt.1.1", verseData);
        SetupLexicon("G1", "Definition 1");
        SetupLexicon("G2", "Definition 2");
        // G3 not in lexicon - should be null

        // Act
        var result = _service.LookupVerses(new[] { "Matt 1:1" });

        // Assert
        Assert.Equal(3, result.Verses[0].Tokens.Count);
        Assert.Equal("Definition 1", result.Verses[0].Tokens[0].LexiconEntry);
        Assert.Equal("Definition 2", result.Verses[0].Tokens[1].LexiconEntry);
        Assert.Null(result.Verses[0].Tokens[2].LexiconEntry);
    }

    #endregion

    #region Helper Methods

    private void SetupNormalizer(string input, string output)
    {
        _mockNormalizer
            .Setup(x => x.TryNormalize(input, out output))
            .Returns(true);
    }

    private void SetupNormalizerFails(string input)
    {
        string? output = null;
        _mockNormalizer
            .Setup(x => x.TryNormalize(input, out output))
            .Returns(false);
    }

    private void SetupVerses(params (string Reference, VerseData Data)[] verses)
    {
        var dict = new ConcurrentDictionary<string, VerseData>();
        foreach (var (reference, data) in verses)
        {
            dict.TryAdd(reference, data);
        }
        _mockBibleDataService.Setup(x => x.Verses).Returns(dict);
    }

    private void SetupVerse(string reference, VerseData data)
    {
        SetupVerses((reference, data));
    }

    private void SetupLexicon(string strongs, string definition)
    {
        var dict = _mockBibleDataService.Object.Lexicon;
        dict.TryAdd(strongs, definition);
    }

    private static VerseData CreateVerseData(string gloss, string strongs)
    {
        return new VerseData
        {
            Tokens = new List<TokenData>
            {
                new()
                {
                    Gloss = gloss,
                    Greek = "Α",
                    Translit = "a",
                    Strongs = strongs,
                    Rmac = "N",
                    RmacDesc = "Noun"
                }
            }
        };
    }

    #endregion
}
