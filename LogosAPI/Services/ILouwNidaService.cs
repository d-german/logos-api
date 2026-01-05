namespace LogosAPI.Services;

/// <summary>
/// Service for looking up Louw-Nida semantic domain labels
/// </summary>
public interface ILouwNidaService
{
    /// <summary>
    /// Gets the human-readable label for a domain code
    /// </summary>
    /// <param name="domainCode">Domain code (e.g., "033005" or "033")</param>
    /// <returns>Human-readable label or null if not found</returns>
    string? GetDomainLabel(string? domainCode);
}
