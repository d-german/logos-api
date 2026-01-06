using System.Collections.Frozen;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service for searching words by Louw-Nida semantic domain
/// </summary>
public sealed class SemanticDomainService : ISemanticDomainService
{
    private const string ResourceName = "LogosAPI.Data.semantic-domains.json";
    
    private readonly FrozenDictionary<int, DomainData> _domains;
    private readonly IReadOnlyList<int> _sortedDomainKeys;
    private readonly ILogger<SemanticDomainService> _logger;

    public SemanticDomainService(ILogger<SemanticDomainService> logger)
    {
        _logger = logger;
        _domains = LoadDomainData();
        _sortedDomainKeys = _domains.Keys.Order().ToList();
        
        _logger.LogInformation(
            "Loaded semantic domains data: {DomainCount} domains, {TotalEntries} total entries",
            _domains.Count,
            _domains.Values.Sum(d => d.EntryCount));
    }

    /// <inheritdoc />
    public DomainSearchResult? GetWordsByDomain(int majorDomain, int maxVersesPerWord = 3)
    {
        if (!_domains.TryGetValue(majorDomain, out var domainData))
            return null;

        var entries = domainData.Entries
            .Select(e => MapToEntry(e, maxVersesPerWord))
            .ToList();

        return new DomainSearchResult
        {
            MajorDomain = majorDomain,
            DomainLabel = domainData.DomainLabel,
            TotalEntries = entries.Count,
            Entries = entries
        };
    }

    /// <inheritdoc />
    public RelatedWordsResult? GetRelatedWords(string louwNidaNumber, int limit = 10, int maxVersesPerWord = 3)
    {
        if (string.IsNullOrWhiteSpace(louwNidaNumber))
            return null;

        // Parse major domain from L-N number (e.g., "57.235" -> 57)
        var (majorDomain, subdomain) = ParseLouwNidaNumber(louwNidaNumber);
        if (majorDomain is null)
            return null;

        if (!_domains.TryGetValue(majorDomain.Value, out var domainData))
            return null;

        // Find the source entry
        var sourceEntry = domainData.Entries
            .FirstOrDefault(e => string.Equals(e.LouwNida, louwNidaNumber, StringComparison.OrdinalIgnoreCase));

        // Get related words (excluding the source)
        var relatedWords = domainData.Entries
            .Where(e => !string.Equals(e.LouwNida, louwNidaNumber, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .Select(e => MapToEntry(e, maxVersesPerWord))
            .ToList();

        return new RelatedWordsResult
        {
            SourceLouwNida = louwNidaNumber,
            SourceLemma = sourceEntry?.Lemma ?? string.Empty,
            MajorDomain = majorDomain.Value,
            DomainLabel = domainData.DomainLabel,
            RelatedWords = relatedWords
        };
    }

    /// <inheritdoc />
    public IReadOnlyList<int> GetAvailableDomains() => _sortedDomainKeys;

    private static DomainEntry MapToEntry(RawDomainEntry raw, int maxVerses)
    {
        return new DomainEntry
        {
            Subdomain = raw.Subdomain,
            SubdomainLabel = raw.SubdomainLabel,
            LouwNida = raw.LouwNida,
            Lemma = raw.Lemma,
            Gloss = raw.Gloss,
            Strongs = raw.Strongs,
            Frequency = raw.Frequency,
            IsHapax = raw.IsHapax,
            SampleVerses = raw.Verses.Take(maxVerses).ToList()
        };
    }

    private static (int? majorDomain, string? subdomain) ParseLouwNidaNumber(string louwNidaNumber)
    {
        var parts = louwNidaNumber.Split('.');
        if (parts.Length < 2)
            return (null, null);

        if (!int.TryParse(parts[0], out var majorDomain))
            return (null, null);

        return (majorDomain, parts[1]);
    }

    private FrozenDictionary<int, DomainData> LoadDomainData()
    {
        var json = ReadEmbeddedResource(ResourceName);
        if (json is null)
        {
            _logger.LogWarning("Failed to load semantic domains resource: {ResourceName}", ResourceName);
            return FrozenDictionary<int, DomainData>.Empty;
        }

        try
        {
            // JSON has string keys like "57", "93", etc.
            var rawData = JsonSerializer.Deserialize<Dictionary<string, DomainData>>(json, JsonOptions);
            if (rawData is null)
            {
                return FrozenDictionary<int, DomainData>.Empty;
            }

            // Convert string keys to int keys
            var result = new Dictionary<int, DomainData>();
            foreach (var (key, value) in rawData)
            {
                if (int.TryParse(key, out var domainNumber))
                {
                    result[domainNumber] = value;
                }
            }

            return result.ToFrozenDictionary();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize semantic domains JSON");
            return FrozenDictionary<int, DomainData>.Empty;
        }
    }

    private static string? ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Internal model for deserializing domain data from JSON
    /// </summary>
    private sealed record DomainData(
        [property: JsonPropertyName("domainLabel")] string DomainLabel,
        [property: JsonPropertyName("entryCount")] int EntryCount,
        [property: JsonPropertyName("entries")] IReadOnlyList<RawDomainEntry> Entries
    );

    /// <summary>
    /// Internal model for deserializing domain entries from JSON
    /// </summary>
    private sealed record RawDomainEntry(
        [property: JsonPropertyName("subdomain")] string Subdomain,
        [property: JsonPropertyName("subdomainLabel")] string? SubdomainLabel,
        [property: JsonPropertyName("louwNida")] string LouwNida,
        [property: JsonPropertyName("lemma")] string Lemma,
        [property: JsonPropertyName("gloss")] string Gloss,
        [property: JsonPropertyName("strongs")] string Strongs,
        [property: JsonPropertyName("frequency")] int Frequency,
        [property: JsonPropertyName("isHapax")] bool IsHapax,
        [property: JsonPropertyName("verses")] IReadOnlyList<string> Verses
    );
}
