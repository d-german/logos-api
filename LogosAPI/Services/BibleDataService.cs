using System.Collections.Concurrent;
using System.Text.Json;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Singleton service that loads and provides access to Bible data
/// Single Responsibility: Load and store Bible verses and lexicon data
/// </summary>
public sealed class BibleDataService : IBibleDataService
{
    private readonly ILogger<BibleDataService> _logger;
    private readonly ConcurrentDictionary<string, VerseData> _verses;
    private readonly ConcurrentDictionary<string, string> _lexicon;
    private readonly bool _isInitialized;

    public ConcurrentDictionary<string, VerseData> Verses => _verses;
    public ConcurrentDictionary<string, string> Lexicon => _lexicon;
    public int VersesCount => _verses.Count;
    public int LexiconCount => _lexicon.Count;
    public bool IsInitialized => _isInitialized;

    public BibleDataService(ILogger<BibleDataService> logger, IWebHostEnvironment env)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = env ?? throw new ArgumentNullException(nameof(env));

        _verses = new ConcurrentDictionary<string, VerseData>();
        _lexicon = new ConcurrentDictionary<string, string>();

        var dataPath = GetDataPath(env.ContentRootPath);
        
        _isInitialized = LoadAllData(dataPath);
    }

    /// <summary>
    /// Gets the data folder path
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static string GetDataPath(string contentRootPath)
    {
        return Path.Combine(contentRootPath, "Data");
    }

    /// <summary>
    /// Loads all data files
    /// Cyclomatic Complexity: 2
    /// </summary>
    private bool LoadAllData(string dataPath)
    {
        var versesLoaded = LoadVerses(dataPath);
        var lexiconLoaded = LoadLexicon(dataPath);

        return versesLoaded && lexiconLoaded;
    }

    /// <summary>
    /// Loads verses from JSON file
    /// Cyclomatic Complexity: 3
    /// </summary>
    private bool LoadVerses(string dataPath)
    {
        var filePath = Path.Combine(dataPath, "verses.json");

        if (!File.Exists(filePath))
        {
            LogFileNotFound("verses.json", filePath);
            return false;
        }

        return LoadVersesFromFile(filePath);
    }

    /// <summary>
    /// Reads and parses verses JSON file
    /// Cyclomatic Complexity: 4
    /// </summary>
    private bool LoadVersesFromFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var options = CreateJsonOptions();
            var data = JsonSerializer.Deserialize<Dictionary<string, VerseData>>(json, options);

            if (data is null)
            {
                LogDeserializationFailed("verses.json");
                return false;
            }

            PopulateVerses(data);
            LogDataLoaded("verses", _verses.Count);
            return true;
        }
        catch (Exception ex)
        {
            LogLoadError("verses.json", ex);
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
    /// Loads lexicon from JSON file
    /// Cyclomatic Complexity: 3
    /// </summary>
    private bool LoadLexicon(string dataPath)
    {
        var filePath = Path.Combine(dataPath, "lexicon.json");

        if (!File.Exists(filePath))
        {
            LogFileNotFound("lexicon.json", filePath);
            return false;
        }

        return LoadLexiconFromFile(filePath);
    }

    /// <summary>
    /// Reads and parses lexicon JSON file
    /// Cyclomatic Complexity: 4
    /// </summary>
    private bool LoadLexiconFromFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var options = CreateJsonOptions();
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, options);

            if (data is null)
            {
                LogDeserializationFailed("lexicon.json");
                return false;
            }

            PopulateLexicon(data);
            LogDataLoaded("lexicon entries", _lexicon.Count);
            return true;
        }
        catch (Exception ex)
        {
            LogLoadError("lexicon.json", ex);
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
    /// Logs file not found
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogFileNotFound(string fileName, string path)
    {
        _logger.LogWarning("Data file {FileName} not found at {Path}", fileName, path);
    }

    /// <summary>
    /// Logs deserialization failure
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogDeserializationFailed(string fileName)
    {
        _logger.LogError("Failed to deserialize {FileName}", fileName);
    }

    /// <summary>
    /// Logs successful data load
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogDataLoaded(string dataType, int count)
    {
        _logger.LogInformation("Loaded {Count} {DataType}", count, dataType);
    }

    /// <summary>
    /// Logs load error with exception
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogLoadError(string fileName, Exception ex)
    {
        _logger.LogError(ex, "Error loading {FileName}", fileName);
    }
}
