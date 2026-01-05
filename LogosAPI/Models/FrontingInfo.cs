namespace LogosAPI.Models;

/// <summary>
/// Represents fronting/marked word order information for a verse
/// </summary>
public sealed class FrontingInfo
{
    /// <summary>
    /// Whether the verse contains any fronting for emphasis
    /// </summary>
    public required bool HasFronting { get; init; }
    
    /// <summary>
    /// Human-readable note explaining the fronting and its exegetical significance
    /// </summary>
    public string? Note { get; init; }
    
    /// <summary>
    /// List of individual clause fronting data (may be null if no details available)
    /// </summary>
    public List<ClauseFrontingData>? Clauses { get; init; }
}
