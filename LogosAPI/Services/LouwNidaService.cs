using System.Collections.Frozen;
using System.Reflection;
using System.Text.Json;

namespace LogosAPI.Services;

/// <summary>
/// Service for looking up Louw-Nida semantic domain labels from embedded resource
/// </summary>
public sealed class LouwNidaService : ILouwNidaService
{
    private const string ResourceName = "LogosAPI.Data.marble-domain-label-mapping.json";
    
    private readonly FrozenDictionary<string, string> _domainLabels;

    public LouwNidaService()
    {
        _domainLabels = LoadDomainLabels();
    }

    /// <inheritdoc />
    public string? GetDomainLabel(string? domainCode)
    {
        if (string.IsNullOrWhiteSpace(domainCode))
            return null;

        // Handle multiple domains (space-separated) - return first match
        var primaryDomain = ExtractPrimaryDomain(domainCode);
        
        return LookupDomain(primaryDomain);
    }

    private static string ExtractPrimaryDomain(string domainCode)
    {
        var spaceIndex = domainCode.IndexOf(' ');
        return spaceIndex > 0 ? domainCode[..spaceIndex] : domainCode;
    }

    private string? LookupDomain(string code)
    {
        // Try exact match first
        if (_domainLabels.TryGetValue(code, out var label))
            return label;

        // Try major domain (first 3 digits)
        var majorDomain = ExtractMajorDomain(code);
        return _domainLabels.GetValueOrDefault(majorDomain);
    }

    private static string ExtractMajorDomain(string code)
    {
        return code.Length >= 3 ? code[..3] : code;
    }

    private static FrozenDictionary<string, string> LoadDomainLabels()
    {
        var json = ReadEmbeddedResource(ResourceName);
        if (json is null)
        {
            return FrozenDictionary<string, string>.Empty;
        }

        var mapping = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
                      ?? new Dictionary<string, string>();
        
        return mapping.ToFrozenDictionary();
    }

    private static string? ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
