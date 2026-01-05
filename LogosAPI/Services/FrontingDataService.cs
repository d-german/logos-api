using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Singleton service that loads and provides access to fronting data from embedded resources
/// Single Responsibility: Load and store Greek word order fronting data
/// </summary>
public sealed class FrontingDataService : IFrontingDataService
{
    private const string FrontingResourceName = "LogosAPI.Data.fronting.json";

    private readonly ILogger<FrontingDataService> _logger;
    private readonly ConcurrentDictionary<string, FrontingInfo> _frontingData;
    private readonly bool _isInitialized;

    public int FrontingCount => _frontingData.Count;
    public bool IsInitialized => _isInitialized;

    public FrontingDataService(ILogger<FrontingDataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _frontingData = new ConcurrentDictionary<string, FrontingInfo>();
        _isInitialized = LoadFrontingData();
    }

    /// <inheritdoc />
    public FrontingInfo? GetFronting(string verseReference)
    {
        if (string.IsNullOrWhiteSpace(verseReference))
        {
            return null;
        }

        return _frontingData.TryGetValue(verseReference, out var frontingInfo) 
            ? frontingInfo 
            : null;
    }

    /// <inheritdoc />
    public bool HasFronting(string verseReference)
    {
        if (string.IsNullOrWhiteSpace(verseReference))
        {
            return false;
        }

        return _frontingData.TryGetValue(verseReference, out var frontingInfo) 
            && frontingInfo.HasFronting;
    }

    /// <summary>
    /// Loads fronting data from embedded resource
    /// Cyclomatic Complexity: 2
    /// </summary>
    private bool LoadFrontingData()
    {
        var json = ReadEmbeddedResource(FrontingResourceName);
        if (json is null)
        {
            LogResourceNotFound(FrontingResourceName);
            return false;
        }

        return ParseAndLoadFronting(json);
    }

    /// <summary>
    /// Parses fronting JSON and loads into dictionary
    /// Cyclomatic Complexity: 4
    /// </summary>
    private bool ParseAndLoadFronting(string json)
    {
        try
        {
            var options = CreateJsonOptions();
            var data = JsonSerializer.Deserialize<Dictionary<string, FrontingInfo>>(json, options);

            if (data is null)
            {
                LogDeserializationFailed("fronting");
                return false;
            }

            PopulateFronting(data);
            LogDataLoaded("fronting entries", _frontingData.Count);
            return true;
        }
        catch (Exception ex)
        {
            LogLoadError("fronting", ex);
            return false;
        }
    }

    /// <summary>
    /// Populates fronting dictionary from parsed data
    /// Cyclomatic Complexity: 2
    /// </summary>
    private void PopulateFronting(Dictionary<string, FrontingInfo> data)
    {
        foreach (var kvp in data)
        {
            _frontingData.TryAdd(kvp.Key, kvp.Value);
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
