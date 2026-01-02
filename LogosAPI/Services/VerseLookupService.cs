using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service for looking up Bible verses and enriching with lexicon data
/// Single Responsibility: Orchestrate verse lookup and lexicon enrichment
/// </summary>
public sealed class VerseLookupService : IVerseLookupService
{
    private readonly IBibleDataService _bibleDataService;
    private readonly IVerseReferenceNormalizer _normalizer;
    private readonly ILogger<VerseLookupService> _logger;

    public VerseLookupService(
        IBibleDataService bibleDataService,
        IVerseReferenceNormalizer normalizer,
        ILogger<VerseLookupService> logger)
    {
        _bibleDataService = bibleDataService ?? throw new ArgumentNullException(nameof(bibleDataService));
        _normalizer = normalizer ?? throw new ArgumentNullException(nameof(normalizer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public VerseLookupResult LookupVerses(IEnumerable<string> references)
    {
        var inputList = references.ToList();
        LogLookupStart(inputList.Count);

        var (normalizedRefs, failedToNormalize) = NormalizeReferences(inputList);
        var (foundVerses, notFoundRefs) = LookupAllVerses(normalizedRefs);
        var allNotFound = CombineNotFound(failedToNormalize, notFoundRefs);

        LogLookupComplete(foundVerses.Count, allNotFound.Count);

        return new VerseLookupResult(foundVerses, allNotFound);
    }

    /// <summary>
    /// Normalizes all references, separating successes from failures
    /// Cyclomatic Complexity: 3
    /// </summary>
    private (List<string> Normalized, List<string> Failed) NormalizeReferences(List<string> references)
    {
        var normalized = new List<string>();
        var failed = new List<string>();

        foreach (var reference in references)
        {
            if (_normalizer.TryNormalize(reference, out var normalizedRef))
            {
                normalized.Add(normalizedRef!);
            }
            else
            {
                failed.Add(reference);
                LogNormalizationFailed(reference);
            }
        }

        return (normalized, failed);
    }

    /// <summary>
    /// Looks up all normalized verses
    /// Cyclomatic Complexity: 3
    /// </summary>
    private (List<VerseResponse> Found, List<string> NotFound) LookupAllVerses(List<string> normalizedRefs)
    {
        var found = new List<VerseResponse>();
        var notFound = new List<string>();

        foreach (var reference in normalizedRefs)
        {
            var verseResponse = LookupSingleVerse(reference);
            if (verseResponse is not null)
            {
                found.Add(verseResponse);
            }
            else
            {
                notFound.Add(reference);
            }
        }

        return (found, notFound);
    }

    /// <summary>
    /// Looks up a single verse and enriches with lexicon data
    /// Cyclomatic Complexity: 2
    /// </summary>
    private VerseResponse? LookupSingleVerse(string reference)
    {
        if (!_bibleDataService.Verses.TryGetValue(reference, out var verseData))
        {
            LogVerseNotFound(reference);
            return null;
        }

        var enrichedTokens = EnrichTokens(verseData.Tokens);
        return new VerseResponse(reference, enrichedTokens);
    }

    /// <summary>
    /// Maps all tokens to response format
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static IReadOnlyList<TokenResponse> EnrichTokens(List<TokenData> tokens)
    {
        return tokens.Select(MapToTokenResponse).ToList();
    }

    /// <summary>
    /// Maps a single token to response format
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static TokenResponse MapToTokenResponse(TokenData token)
    {
        return new TokenResponse(
            token.Gloss,
            token.Greek,
            token.Translit,
            token.Strongs,
            token.Rmac,
            token.RmacDesc
        );
    }

    /// <summary>
    /// Combines failed normalizations with not-found verses
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static IReadOnlyList<string> CombineNotFound(List<string> failed, List<string> notFound)
    {
        return failed.Concat(notFound).ToList();
    }

    /// <summary>
    /// Logs the start of a lookup operation
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogLookupStart(int count)
    {
        _logger.LogInformation("Starting verse lookup for {Count} references", count);
    }

    /// <summary>
    /// Logs completion of a lookup operation
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogLookupComplete(int foundCount, int notFoundCount)
    {
        _logger.LogInformation("Verse lookup complete. Found: {Found}, NotFound: {NotFound}", foundCount, notFoundCount);
    }

    /// <summary>
    /// Logs a normalization failure
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogNormalizationFailed(string reference)
    {
        _logger.LogWarning("Failed to normalize verse reference: {Reference}", reference);
    }

    /// <summary>
    /// Logs a verse not found
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogVerseNotFound(string reference)
    {
        _logger.LogWarning("Verse not found in dictionary: {Reference}", reference);
    }
}
