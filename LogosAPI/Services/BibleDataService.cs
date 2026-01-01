using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Singleton service that loads and provides access to Bible data from embedded resources
/// Single Responsibility: Load and store Bible verses and lexicon data
/// </summary>
public sealed class BibleDataService : IBibleDataService
{
    private const string VersesResourceName = "LogosAPI.Data.verses.json";
    private const string LexiconResourceName = "LogosAPI.Data.lexicon.json";

    private readonly ILogger<BibleDataService> _logger;
    private readonly ConcurrentDictionary<string, VerseData> _verses;
    private readonly ConcurrentDictionary<string, string> _lexicon;
    private readonly bool _isInitialized;

    public ConcurrentDictionary<string, VerseData> Verses => _verses;
    public ConcurrentDictionary<string, string> Lexicon => _lexicon;
    public int VersesCount => _verses.Count;
    public int LexiconCount => _lexicon.Count;
    public bool IsInitialized => _isInitialized;

    public BibleDataService(ILogger<BibleDataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _verses = new ConcurrentDictionary<string, VerseData>();
        _lexicon = new ConcurrentDictionary<string, string>();

        _isInitialized = LoadAllData();
    }

    /// <summary>
    /// Loads all data from embedded resources
    /// Cyclomatic Complexity: 2
    /// </summary>
    private bool LoadAllData()
    {
        var versesLoaded = LoadVerses();
        var lexiconLoaded = LoadLexicon();

        return versesLoaded && lexiconLoaded;
    }

    /// <summary>
    /// Loads verses from embedded resource
    /// Cyclomatic Complexity: 2
    /// </summary>
    private bool LoadVerses()
    {
        var json = ReadEmbeddedResource(VersesResourceName);
        if (json is null)
        {
            LogResourceNotFound(VersesResourceName);
            return false;
        }

        return ParseAndLoadVerses(json);
    }

    /// <summary>
    /// Parses verses JSON and loads into dictionary
    /// Cyclomatic Complexity: 4
    /// </summary>
    private bool ParseAndLoadVerses(string json)
    {
        try
        {
            var options = CreateJsonOptions();
            var data = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(json, options);

            if (data is null)
            {
                LogDeserializationFailed("verses");
                return false;
            }

            PopulateVerses(data);
            LogDataLoaded("verses", _verses.Count);
            return true;
        }
        catch (Exception ex)
        {
            LogLoadError("verses", ex);
            return false;
        }
    }

    /// <summary>
    /// Populates verses dictionary from parsed data
    /// Cyclomatic Complexity: 2
    /// </summary>
    private void PopulateVerses(Dictionary<string, VerseData> data)
    {
        foreach (var kvp in data)
        {
            _verses.TryAdd(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Loads lexicon from embedded resource
    /// Cyclomatic Complexity: 2
    /// </summary>
    private bool LoadLexicon()
    {
        var json = ReadEmbeddedResource(LexiconResourceName);
        if (json is null)
        {
            LogResourceNotFound(LexiconResourceName);
            return false;
        }

        return ParseAndLoadLexicon(json);
    }

    /// <summary>
    /// Parses lexicon JSON and loads into dictionary
    /// Cyclomatic Complexity: 4
    /// </summary>
    private bool ParseAndLoadLexicon(string json)
    {
        try
        {
            var options = CreateJsonOptions();
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, options);

            if (data is null)
            {
                LogDeserializationFailed("lexicon");
                return false;
            }

            PopulateLexicon(data);
            LogDataLoaded("lexicon entries", _lexicon.Count);
            return true;
        }
        catch (Exception ex)
        {
            LogLoadError("lexicon", ex);
            return false;
        }
    }

    /// <summary>
    /// Populates lexicon dictionary from parsed data
    /// Cyclomatic Complexity: 2
    /// </summary>
    private void PopulateLexicon(Dictionary<string, string> data)
    {
        foreach (var kvp in data)
        {
            _lexicon.TryAdd(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Reads an embedded resource as string
    /// Cyclomatic Complexity: 3
    /// </summary>
    private string? ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            LogAvailableResources(assembly);
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Logs available embedded resources for debugging
    /// Cyclomatic Complexity: 2
    /// </summary>
    private void LogAvailableResources(Assembly assembly)
    {
        var resources = assembly.GetManifestResourceNames();
        _logger.LogWarning(
            "Available embedded resources: {Resources}",
            string.Join(", ", resources));
    }

    /// <summary>
    /// Creates JSON serializer options
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static JsonSerializerOptions CreateJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Logs resource not found
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogResourceNotFound(string resourceName)
    {
        _logger.LogWarning("Embedded resource not found: {ResourceName}", resourceName);
    }

    /// <summary>
    /// Logs deserialization failure
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogDeserializationFailed(string dataType)
    {
        _logger.LogError("Failed to deserialize {DataType} data", dataType);
    }

    /// <summary>
    /// Logs successful data load
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogDataLoaded(string dataType, int count)
    {
        _logger.LogInformation("Loaded {Count} {DataType} from embedded resources", count, dataType);
    }

    /// <summary>
    /// Logs load error with exception
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogLoadError(string dataType, Exception ex)
    {
        _logger.LogError(ex, "Error loading {DataType} from embedded resource", dataType);
    }
}
