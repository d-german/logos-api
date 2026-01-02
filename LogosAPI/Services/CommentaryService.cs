using System.Text.Json;
using System.Text.Json.Serialization;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service for retrieving Bible commentary from HelloAO API (https://bible.helloao.org/)
/// </summary>
public sealed class CommentaryService : ICommentaryService
{
    private const string BaseUrl = "https://bible.helloao.org/api";
    
    private readonly HttpClient _httpClient;
    private readonly ILogger<CommentaryService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CommentaryService(HttpClient httpClient, ILogger<CommentaryService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = CreateJsonOptions();
    }

    /// <inheritdoc />
    public async Task<AvailableCommentariesResponse> GetAvailableCommentariesAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/available_commentaries.json";
        _logger.LogInformation("Fetching available commentaries from {Url}", url);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var helloAoResponse = JsonSerializer.Deserialize<HelloAoCommentariesResponse>(json, _jsonOptions);

        if (helloAoResponse?.Commentaries is null)
        {
            return new AvailableCommentariesResponse([]);
        }

        var commentaries = helloAoResponse.Commentaries
            .Select(MapToCommentaryInfo)
            .ToList();

        return new AvailableCommentariesResponse(commentaries);
    }

    /// <inheritdoc />
    public async Task<CommentaryLookupResponse?> GetCommentaryAsync(
        string commentaryId,
        string book,
        int chapter,
        int? verse = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedBook = book.ToUpperInvariant();
        var url = $"{BaseUrl}/c/{commentaryId}/{normalizedBook}/{chapter}.json";
        
        _logger.LogDebug("Fetching commentary from {Url}", url);

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Commentary not found: {StatusCode} for {Url}", response.StatusCode, url);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            
            // Check if response is actually JSON (HelloAO sometimes returns HTML error pages)
            if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith('{'))
            {
                _logger.LogDebug("Non-JSON response received from {Url}", url);
                return null;
            }

            var chapterResponse = JsonSerializer.Deserialize<HelloAoChapterResponse>(json, _jsonOptions);

            if (chapterResponse is null)
            {
                return null;
            }

            var reference = verse.HasValue 
                ? $"{normalizedBook}.{chapter}.{verse}" 
                : $"{normalizedBook}.{chapter}";

            var verseCommentaries = ExtractVerseCommentaries(chapterResponse, verse);
            var notFound = DetermineNotFound(verse, verseCommentaries);

            return new CommentaryLookupResponse(
                reference,
                chapterResponse.Commentary?.Id ?? commentaryId,
                chapterResponse.Commentary?.Name ?? commentaryId,
                verseCommentaries,
                notFound
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogDebug(ex, "HTTP error fetching commentary from {Url}", url);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogDebug(ex, "JSON parsing error for commentary from {Url}", url);
            return null;
        }
    }

    /// <summary>
    /// Extracts verse commentaries from the chapter response
    /// </summary>
    private static IReadOnlyList<VerseCommentary> ExtractVerseCommentaries(
        HelloAoChapterResponse chapterResponse, 
        int? targetVerse)
    {
        if (chapterResponse.Chapter?.Content is null)
        {
            return [];
        }

        var verses = chapterResponse.Chapter.Content
            .Where(c => c.Type == "verse")
            .Where(c => !targetVerse.HasValue || c.Number == targetVerse.Value)
            .Select(MapToVerseCommentary)
            .Where(v => v is not null)
            .Cast<VerseCommentary>()
            .ToList();

        return verses;
    }

    /// <summary>
    /// Maps HelloAO content to VerseCommentary
    /// </summary>
    private static VerseCommentary? MapToVerseCommentary(HelloAoContent content)
    {
        if (content.Number is null || content.Content is null)
        {
            return null;
        }

        var textContent = string.Join(" ", content.Content
            .Where(c => c.ValueKind == JsonValueKind.String)
            .Select(c => c.GetString() ?? ""));

        return new VerseCommentary(content.Number.Value, textContent.Trim());
    }

    /// <summary>
    /// Determines which verses were not found
    /// </summary>
    private static IReadOnlyList<string> DetermineNotFound(int? targetVerse, IReadOnlyList<VerseCommentary> found)
    {
        if (!targetVerse.HasValue)
        {
            return [];
        }

        return found.Any(v => v.VerseNumber == targetVerse.Value) 
            ? [] 
            : [$"Verse {targetVerse.Value}"];
    }

    /// <summary>
    /// Maps HelloAO commentary to our CommentaryInfo model
    /// </summary>
    private static CommentaryInfo MapToCommentaryInfo(HelloAoCommentary c) => new(
        c.Id ?? "",
        c.Name ?? "",
        c.EnglishName ?? c.Name ?? "",
        c.Website ?? "",
        c.LicenseUrl ?? "",
        c.LicenseNotes,
        c.NumberOfBooks,
        c.TotalNumberOfChapters,
        c.TotalNumberOfVerses
    );

    /// <summary>
    /// Creates JSON serializer options
    /// </summary>
    private static JsonSerializerOptions CreateJsonOptions() => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <inheritdoc />
    public async Task<AllCommentariesLookupResponse> GetAllCommentariesAsync(
        IReadOnlyList<(string Book, int Chapter, int Verse, string OriginalRef)> parsedReferences,
        CancellationToken cancellationToken = default)
    {
        // Get available commentaries first
        var availableCommentaries = await GetAvailableCommentariesAsync(cancellationToken);
        var commentaryIds = availableCommentaries.Commentaries.Select(c => c.Id).ToList();

        var results = new List<CommentaryResult>();
        var allNotFound = new List<string>();
        var references = parsedReferences.Select(r => r.OriginalRef).ToList();

        // Process each commentary in parallel
        var tasks = commentaryIds.Select(async commentaryId =>
        {
            var entries = new List<VerseCommentaryEntry>();

            foreach (var (book, chapter, verse, originalRef) in parsedReferences)
            {
                var commentary = await GetCommentaryAsync(commentaryId, book, chapter, verse, cancellationToken);
                
                if (commentary?.Verses.Count > 0)
                {
                    var verseContent = commentary.Verses.FirstOrDefault(v => v.VerseNumber == verse);
                    // Only add entries that have actual content
                    if (!string.IsNullOrEmpty(verseContent?.Content))
                    {
                        entries.Add(new VerseCommentaryEntry(
                            originalRef,
                            verse,
                            verseContent.Content
                        ));
                    }
                }
            }

            var commentaryInfo = availableCommentaries.Commentaries.FirstOrDefault(c => c.Id == commentaryId);
            return new CommentaryResult(
                commentaryId,
                commentaryInfo?.Name ?? commentaryId,
                entries
            );
        });

        var commentaryResults = await Task.WhenAll(tasks);
        
        // Only include commentaries that have at least one entry with content
        results.AddRange(commentaryResults.Where(r => r.Entries.Count > 0));

        // Determine not found (references that have no commentary in ANY source)
        foreach (var (_, _, verse, originalRef) in parsedReferences)
        {
            var hasAnyCommentary = results.Any(r => 
                r.Entries.Any(e => e.Reference == originalRef && !string.IsNullOrEmpty(e.Content)));
            
            if (!hasAnyCommentary)
            {
                allNotFound.Add(originalRef);
            }
        }

        return new AllCommentariesLookupResponse(references, results, allNotFound);
    }

    #region HelloAO API DTOs

    private sealed record HelloAoCommentariesResponse(
        [property: JsonPropertyName("commentaries")] List<HelloAoCommentary>? Commentaries
    );

    private sealed record HelloAoCommentary(
        [property: JsonPropertyName("id")] string? Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("englishName")] string? EnglishName,
        [property: JsonPropertyName("website")] string? Website,
        [property: JsonPropertyName("licenseUrl")] string? LicenseUrl,
        [property: JsonPropertyName("licenseNotes")] string? LicenseNotes,
        [property: JsonPropertyName("numberOfBooks")] int NumberOfBooks,
        [property: JsonPropertyName("totalNumberOfChapters")] int TotalNumberOfChapters,
        [property: JsonPropertyName("totalNumberOfVerses")] int TotalNumberOfVerses
    );

    private sealed record HelloAoChapterResponse(
        [property: JsonPropertyName("commentary")] HelloAoCommentary? Commentary,
        [property: JsonPropertyName("chapter")] HelloAoChapter? Chapter
    );

    private sealed record HelloAoChapter(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("content")] List<HelloAoContent>? Content
    );

    private sealed record HelloAoContent(
        [property: JsonPropertyName("type")] string? Type,
        [property: JsonPropertyName("number")] int? Number,
        [property: JsonPropertyName("content")] List<JsonElement>? Content
    );

    #endregion
}
