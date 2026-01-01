using LogosAPI.Services;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for VerseReferenceNormalizer
/// </summary>
public sealed class VerseReferenceNormalizerTests
{
    private readonly IVerseReferenceNormalizer _normalizer = new VerseReferenceNormalizer();

    #region Valid Input Tests

    [Theory]
    [InlineData("Matt.1.1", "Matt.1.1")]
    [InlineData("Matthew 1:1", "Matt.1.1")]
    [InlineData("Matt 1:1", "Matt.1.1")]
    [InlineData("matt.1.1", "Matt.1.1")]
    [InlineData("MATT.1.1", "Matt.1.1")]
    [InlineData("Mt 1:1", "Matt.1.1")]
    [InlineData("John 3:16", "John.3.16")]
    [InlineData("Jn 3:16", "John.3.16")]
    [InlineData("jhn 3:16", "John.3.16")]
    public void Normalize_SingleWordBook_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1 Cor 13:4", "1Cor.13.4")]
    [InlineData("1Cor 13:4", "1Cor.13.4")]
    [InlineData("1 Corinthians 13:4", "1Cor.13.4")]
    [InlineData("I Corinthians 13:4", "1Cor.13.4")]
    [InlineData("First Corinthians 13:4", "1Cor.13.4")]
    [InlineData("2 Cor 5:17", "2Cor.5.17")]
    [InlineData("II Corinthians 5:17", "2Cor.5.17")]
    [InlineData("Second Corinthians 5:17", "2Cor.5.17")]
    public void Normalize_NumberedBook_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1 Pet 2:9", "1Pet.2.9")]
    [InlineData("2 Pet 3:9", "2Pet.3.9")]
    [InlineData("1 John 4:8", "1John.4.8")]
    [InlineData("2 John 1:3", "2John.1.3")]
    [InlineData("3 John 1:4", "3John.1.4")]
    [InlineData("III John 1:4", "3John.1.4")]
    [InlineData("Third John 1:4", "3John.1.4")]
    [InlineData("1 Thess 5:18", "1Thess.5.18")]
    [InlineData("2 Thess 3:3", "2Thess.3.3")]
    [InlineData("1 Tim 6:12", "1Tim.6.12")]
    [InlineData("2 Tim 2:15", "2Tim.2.15")]
    public void Normalize_NumberedEpistles_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Rom 8:28", "Rom.8.28")]
    [InlineData("Romans 8:28", "Rom.8.28")]
    [InlineData("Gal 5:22", "Gal.5.22")]
    [InlineData("Galatians 5:22", "Gal.5.22")]
    [InlineData("Eph 2:8", "Eph.2.8")]
    [InlineData("Ephesians 2:8", "Eph.2.8")]
    [InlineData("Phil 4:13", "Phil.4.13")]
    [InlineData("Philippians 4:13", "Phil.4.13")]
    [InlineData("Col 3:23", "Col.3.23")]
    [InlineData("Colossians 3:23", "Col.3.23")]
    public void Normalize_PaulineEpistles_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Heb 11:1", "Heb.11.1")]
    [InlineData("Hebrews 11:1", "Heb.11.1")]
    [InlineData("Jas 1:2", "Jas.1.2")]
    [InlineData("James 1:2", "Jas.1.2")]
    [InlineData("Jude 1:3", "Jude.1.3")]
    [InlineData("Rev 22:21", "Rev.22.21")]
    [InlineData("Revelation 22:21", "Rev.22.21")]
    [InlineData("Revelations 22:21", "Rev.22.21")]
    public void Normalize_GeneralEpistles_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Titus 2:11", "Titus.2.11")]
    [InlineData("Tit 2:11", "Titus.2.11")]
    [InlineData("Philemon 1:6", "Phlm.1.6")]
    [InlineData("Phlm 1:6", "Phlm.1.6")]
    public void Normalize_ShortBooks_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Delimiter Variation Tests

    [Theory]
    [InlineData("Matt 1:1", "Matt.1.1")]
    [InlineData("Matt 1.1", "Matt.1.1")]
    [InlineData("Matt.1.1", "Matt.1.1")]
    [InlineData("Matt.1:1", "Matt.1.1")]
    [InlineData("Matt 1-1", "Matt.1.1")]
    public void Normalize_DifferentDelimiters_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Edge Case Tests

    [Theory]
    [InlineData("Matt.01.01", "Matt.1.1")]
    [InlineData("Matt.001.001", "Matt.1.1")]
    public void Normalize_LeadingZeros_RemovesZeros(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  Matt 1:1  ", "Matt.1.1")]
    [InlineData("Matt  1:1", "Matt.1.1")]
    [InlineData("Matt 1 : 1", "Matt.1.1")]
    public void Normalize_ExtraWhitespace_HandlesCorrectly(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("MATTHEW 1:1", "Matt.1.1")]
    [InlineData("matthew 1:1", "Matt.1.1")]
    [InlineData("MaTtHeW 1:1", "Matt.1.1")]
    public void Normalize_CaseVariations_HandlesCorrectly(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Invalid Input Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Normalize_EmptyOrNull_ThrowsArgumentException(string? input)
    {
        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(input!));
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("Matt")]
    [InlineData("Matt 1")]
    [InlineData("Unknown 1:1")]
    [InlineData("1:1")]
    [InlineData("Hello World")]
    public void Normalize_InvalidFormat_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(input));
    }

    #endregion

    #region TryNormalize Tests

    [Fact]
    public void TryNormalize_ValidInput_ReturnsTrue()
    {
        var result = _normalizer.TryNormalize("Matt 1:1", out var normalized);

        Assert.True(result);
        Assert.Equal("Matt.1.1", normalized);
    }

    [Fact]
    public void TryNormalize_InvalidInput_ReturnsFalse()
    {
        var result = _normalizer.TryNormalize("Invalid", out var normalized);

        Assert.False(result);
        Assert.Null(normalized);
    }

    [Fact]
    public void TryNormalize_NullInput_ReturnsFalse()
    {
        var result = _normalizer.TryNormalize(null!, out var normalized);

        Assert.False(result);
        Assert.Null(normalized);
    }

    #endregion

    #region IsValid Tests

    [Theory]
    [InlineData("Matt 1:1", true)]
    [InlineData("John 3:16", true)]
    [InlineData("1 Cor 13:4", true)]
    [InlineData("Invalid", false)]
    [InlineData("", false)]
    public void IsValid_VariousInputs_ReturnsCorrectResult(string input, bool expected)
    {
        var result = _normalizer.IsValid(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region All Books Coverage Tests

    [Theory]
    [InlineData("Matt 1:1", "Matt.1.1")]
    [InlineData("Mark 1:1", "Mark.1.1")]
    [InlineData("Luke 1:1", "Luke.1.1")]
    [InlineData("John 1:1", "John.1.1")]
    [InlineData("Acts 1:1", "Acts.1.1")]
    [InlineData("Rom 1:1", "Rom.1.1")]
    [InlineData("1 Cor 1:1", "1Cor.1.1")]
    [InlineData("2 Cor 1:1", "2Cor.1.1")]
    [InlineData("Gal 1:1", "Gal.1.1")]
    [InlineData("Eph 1:1", "Eph.1.1")]
    [InlineData("Phil 1:1", "Phil.1.1")]
    [InlineData("Col 1:1", "Col.1.1")]
    [InlineData("1 Thess 1:1", "1Thess.1.1")]
    [InlineData("2 Thess 1:1", "2Thess.1.1")]
    [InlineData("1 Tim 1:1", "1Tim.1.1")]
    [InlineData("2 Tim 1:1", "2Tim.1.1")]
    [InlineData("Titus 1:1", "Titus.1.1")]
    [InlineData("Phlm 1:1", "Phlm.1.1")]
    [InlineData("Heb 1:1", "Heb.1.1")]
    [InlineData("Jas 1:1", "Jas.1.1")]
    [InlineData("1 Pet 1:1", "1Pet.1.1")]
    [InlineData("2 Pet 1:1", "2Pet.1.1")]
    [InlineData("1 John 1:1", "1John.1.1")]
    [InlineData("2 John 1:1", "2John.1.1")]
    [InlineData("3 John 1:1", "3John.1.1")]
    [InlineData("Jude 1:1", "Jude.1.1")]
    [InlineData("Rev 1:1", "Rev.1.1")]
    public void Normalize_AllBooks_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    #endregion
}
