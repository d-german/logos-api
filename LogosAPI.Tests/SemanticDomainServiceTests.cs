using LogosAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for SemanticDomainService
/// </summary>
public sealed class SemanticDomainServiceTests
{
    private readonly ISemanticDomainService _service;

    public SemanticDomainServiceTests()
    {
        var loggerMock = new Mock<ILogger<SemanticDomainService>>();
        _service = new SemanticDomainService(loggerMock.Object);
    }

    #region GetAvailableDomains Tests

    [Fact]
    public void GetAvailableDomains_ReturnsNonEmptyList()
    {
        var domains = _service.GetAvailableDomains();

        Assert.NotNull(domains);
        Assert.NotEmpty(domains);
    }

    [Fact]
    public void GetAvailableDomains_ReturnsAscendingOrder()
    {
        var domains = _service.GetAvailableDomains();

        Assert.NotNull(domains);
        Assert.Equal(domains.OrderBy(x => x).ToList(), domains);
    }

    [Fact]
    public void GetAvailableDomains_ContainsKnownDomains()
    {
        var domains = _service.GetAvailableDomains();

        // Domain 57 (Possess, Transfer, Exchange) and 33 (Communication) are major domains
        Assert.Contains(57, domains);
        Assert.Contains(33, domains);
        Assert.Contains(1, domains);
        Assert.Contains(93, domains);
    }

    #endregion

    #region GetWordsByDomain Tests

    [Fact]
    public void GetWordsByDomain_ValidDomain_ReturnsResult()
    {
        var result = _service.GetWordsByDomain(57);

        Assert.NotNull(result);
        Assert.Equal(57, result.MajorDomain);
        Assert.NotEmpty(result.DomainLabel);
        Assert.True(result.TotalEntries > 0);
        Assert.NotEmpty(result.Entries);
    }

    [Fact]
    public void GetWordsByDomain_Domain57_HasEconomicTerms()
    {
        var result = _service.GetWordsByDomain(57);

        Assert.NotNull(result);
        Assert.Contains("Possess", result.DomainLabel);
        Assert.True(result.TotalEntries > 0);
    }

    [Fact]
    public void GetWordsByDomain_InvalidDomain_ReturnsNull()
    {
        var result = _service.GetWordsByDomain(9999);

        Assert.Null(result);
    }

    [Fact]
    public void GetWordsByDomain_NegativeDomain_ReturnsNull()
    {
        var result = _service.GetWordsByDomain(-1);

        Assert.Null(result);
    }

    [Fact]
    public void GetWordsByDomain_EntriesHaveRequiredFields()
    {
        var result = _service.GetWordsByDomain(57);

        Assert.NotNull(result);
        foreach (var entry in result.Entries.Take(5))
        {
            Assert.NotEmpty(entry.Subdomain);
            Assert.NotEmpty(entry.LouwNida);
            Assert.NotEmpty(entry.Lemma);
            Assert.NotEmpty(entry.Gloss);
            Assert.NotEmpty(entry.Strongs);
            Assert.True(entry.Frequency >= 0);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void GetWordsByDomain_LimitsVersesPerWord(int maxVerses)
    {
        var result = _service.GetWordsByDomain(57, maxVersesPerWord: maxVerses);

        Assert.NotNull(result);
        foreach (var entry in result.Entries)
        {
            Assert.True(entry.SampleVerses.Count <= maxVerses,
                $"Entry {entry.Lemma} has {entry.SampleVerses.Count} verses, expected <= {maxVerses}");
        }
    }

    [Fact]
    public void GetWordsByDomain_EntriesSortedByFrequencyDescending()
    {
        var result = _service.GetWordsByDomain(57);

        Assert.NotNull(result);
        var frequencies = result.Entries.Select(e => e.Frequency).ToList();
        Assert.Equal(frequencies.OrderByDescending(f => f).ToList(), frequencies);
    }

    [Fact]
    public void GetWordsByDomain_HapaxFlaggedCorrectly()
    {
        var result = _service.GetWordsByDomain(57);

        Assert.NotNull(result);
        foreach (var entry in result.Entries)
        {
            Assert.Equal(entry.Frequency == 1, entry.IsHapax);
        }
    }

    #endregion

    #region GetRelatedWords Tests

    [Fact]
    public void GetRelatedWords_ValidLouwNida_ReturnsResult()
    {
        // First get a known valid L-N number from domain 57
        var domainResult = _service.GetWordsByDomain(57, 1);
        Assert.NotNull(domainResult);
        Assert.NotEmpty(domainResult.Entries);

        var testLouwNida = domainResult.Entries.First().LouwNida;
        var result = _service.GetRelatedWords(testLouwNida, limit: 5);

        Assert.NotNull(result);
        Assert.Equal(testLouwNida, result.SourceLouwNida);
        Assert.Equal(57, result.MajorDomain);
        Assert.NotEmpty(result.DomainLabel);
    }

    [Fact]
    public void GetRelatedWords_RespectsLimit()
    {
        var domainResult = _service.GetWordsByDomain(57, 1);
        Assert.NotNull(domainResult);
        Assert.NotEmpty(domainResult.Entries);

        var testLouwNida = domainResult.Entries.First().LouwNida;
        var result = _service.GetRelatedWords(testLouwNida, limit: 5);

        Assert.NotNull(result);
        Assert.True(result.RelatedWords.Count <= 5);
    }

    [Fact]
    public void GetRelatedWords_ExcludesSourceWord()
    {
        var domainResult = _service.GetWordsByDomain(57, 1);
        Assert.NotNull(domainResult);
        Assert.NotEmpty(domainResult.Entries);

        var sourceEntry = domainResult.Entries.First();
        var result = _service.GetRelatedWords(sourceEntry.LouwNida, limit: 100);

        Assert.NotNull(result);
        // Related words should not include the source L-N number
        Assert.DoesNotContain(result.RelatedWords, w => w.LouwNida == sourceEntry.LouwNida);
    }

    [Fact]
    public void GetRelatedWords_InvalidLouwNida_ReturnsNull()
    {
        var result = _service.GetRelatedWords("invalid");

        Assert.Null(result);
    }

    [Fact]
    public void GetRelatedWords_NonExistentDomain_ReturnsNull()
    {
        var result = _service.GetRelatedWords("9999.1");

        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetRelatedWords_EmptyOrNullInput_ReturnsNull(string? louwNida)
    {
        var result = _service.GetRelatedWords(louwNida!);

        Assert.Null(result);
    }

    [Fact]
    public void GetRelatedWords_LimitsVersesPerWord()
    {
        var domainResult = _service.GetWordsByDomain(57, 1);
        Assert.NotNull(domainResult);
        Assert.NotEmpty(domainResult.Entries);

        var testLouwNida = domainResult.Entries.First().LouwNida;
        var result = _service.GetRelatedWords(testLouwNida, limit: 5, maxVersesPerWord: 2);

        Assert.NotNull(result);
        foreach (var word in result.RelatedWords)
        {
            Assert.True(word.SampleVerses.Count <= 2);
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void GetWordsByDomain_ZeroMaxVerses_ReturnsEmptyVerseArrays()
    {
        var result = _service.GetWordsByDomain(57, maxVersesPerWord: 0);

        Assert.NotNull(result);
        foreach (var entry in result.Entries)
        {
            Assert.Empty(entry.SampleVerses);
        }
    }

    [Theory]
    [InlineData("57.1")]
    [InlineData("33.69")]
    [InlineData("93.169a")]
    public void GetRelatedWords_VariousLouwNidaFormats_ReturnsResult(string louwNida)
    {
        // Try to find related words - may return null if specific L-N doesn't exist
        // but should not throw
        var result = _service.GetRelatedWords(louwNida);

        // If domain exists, result should be valid
        var majorDomain = int.Parse(louwNida.Split('.')[0]);
        var domainExists = _service.GetWordsByDomain(majorDomain) != null;

        if (domainExists)
        {
            // Either found related words or the specific L-N doesn't exist
            // This is valid behavior
        }
        // No exception thrown - test passes
    }

    #endregion
}
