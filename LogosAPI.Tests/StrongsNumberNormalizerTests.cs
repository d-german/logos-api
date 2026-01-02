using LogosAPI.Services;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for StrongsNumberNormalizer
/// </summary>
public sealed class StrongsNumberNormalizerTests
{
    private readonly IStrongsNumberNormalizer _normalizer = new StrongsNumberNormalizer();

    #region Valid Input Tests - Greek (G prefix)

    [Theory]
    [InlineData("G25", "G25")]
    [InlineData("g25", "G25")]
    [InlineData("G 25", "G25")]
    [InlineData("G0025", "G25")]
    [InlineData("g 0025", "G25")]
    [InlineData("G976", "G976")]
    [InlineData("G5547", "G5547")]
    public void Normalize_GreekStrongsNumber_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Valid Input Tests - Hebrew (H prefix)

    [Theory]
    [InlineData("H1234", "H1234")]
    [InlineData("h1234", "H1234")]
    [InlineData("H 1234", "H1234")]
    [InlineData("H0001", "H1")]
    [InlineData("h 0001", "H1")]
    [InlineData("H430", "H430")]
    public void Normalize_HebrewStrongsNumber_ReturnsCanonical(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Edge Case Tests

    [Theory]
    [InlineData("  G25  ", "G25")]
    [InlineData("G  25", "G25")]
    [InlineData("  H1234  ", "H1234")]
    public void Normalize_ExtraWhitespace_HandlesCorrectly(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("G0", "G0")]
    [InlineData("H0", "H0")]
    [InlineData("G00", "G0")]
    [InlineData("G000", "G0")]
    public void Normalize_ZeroNumber_ReturnsZero(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("G1", "G1")]
    [InlineData("G01", "G1")]
    [InlineData("G001", "G1")]
    [InlineData("G0001", "G1")]
    public void Normalize_LeadingZeros_RemovesZeros(string input, string expected)
    {
        var result = _normalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("G25", "G25")]
    [InlineData("g25", "G25")]
    [InlineData("H1234", "H1234")]
    [InlineData("h1234", "H1234")]
    public void Normalize_CaseVariations_ReturnsUppercasePrefix(string input, string expected)
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
    [InlineData("invalid")]
    [InlineData("X25")]
    [InlineData("A1234")]
    [InlineData("G")]
    [InlineData("H")]
    [InlineData("25")]
    [InlineData("1234")]
    [InlineData("GH25")]
    [InlineData("G25H")]
    [InlineData("Hello")]
    [InlineData("G-25")]
    [InlineData("G.25")]
    public void Normalize_InvalidFormat_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => _normalizer.Normalize(input));
    }

    #endregion

    #region TryNormalize Tests

    [Fact]
    public void TryNormalize_ValidGreekInput_ReturnsTrueWithNormalized()
    {
        var result = _normalizer.TryNormalize("G25", out var normalized);

        Assert.True(result);
        Assert.Equal("G25", normalized);
    }

    [Fact]
    public void TryNormalize_ValidHebrewInput_ReturnsTrueWithNormalized()
    {
        var result = _normalizer.TryNormalize("H1234", out var normalized);

        Assert.True(result);
        Assert.Equal("H1234", normalized);
    }

    [Fact]
    public void TryNormalize_LowercaseWithLeadingZeros_ReturnsTrueWithNormalized()
    {
        var result = _normalizer.TryNormalize("g 0025", out var normalized);

        Assert.True(result);
        Assert.Equal("G25", normalized);
    }

    [Fact]
    public void TryNormalize_InvalidInput_ReturnsFalseWithNull()
    {
        var result = _normalizer.TryNormalize("invalid", out var normalized);

        Assert.False(result);
        Assert.Null(normalized);
    }

    [Fact]
    public void TryNormalize_NullInput_ReturnsFalseWithNull()
    {
        var result = _normalizer.TryNormalize(null!, out var normalized);

        Assert.False(result);
        Assert.Null(normalized);
    }

    [Fact]
    public void TryNormalize_EmptyInput_ReturnsFalseWithNull()
    {
        var result = _normalizer.TryNormalize("", out var normalized);

        Assert.False(result);
        Assert.Null(normalized);
    }

    [Fact]
    public void TryNormalize_WhitespaceInput_ReturnsFalseWithNull()
    {
        var result = _normalizer.TryNormalize("   ", out var normalized);

        Assert.False(result);
        Assert.Null(normalized);
    }

    #endregion

    #region IsValid Tests

    [Theory]
    [InlineData("G25", true)]
    [InlineData("g25", true)]
    [InlineData("H1234", true)]
    [InlineData("h1234", true)]
    [InlineData("G 0025", true)]
    [InlineData("invalid", false)]
    [InlineData("X25", false)]
    [InlineData("", false)]
    [InlineData("G", false)]
    [InlineData("25", false)]
    public void IsValid_VariousInputs_ReturnsCorrectBoolean(string input, bool expected)
    {
        var result = _normalizer.IsValid(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsValid_NullInput_ReturnsFalse()
    {
        var result = _normalizer.IsValid(null!);
        Assert.False(result);
    }

    #endregion
}
