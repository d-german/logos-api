using LogosAPI.Models;
using LogosAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogosAPI.Controllers;

/// <summary>
/// Controller for Louw-Nida semantic domain search operations.
/// Enables finding thematically related Greek words by semantic domain.
/// </summary>
[ApiController]
[Route("api/semantic-domain")]
public sealed class SemanticDomainController : ControllerBase
{
    private readonly ILogger<SemanticDomainController> _logger;
    private readonly ISemanticDomainService _domainService;

    public SemanticDomainController(
        ILogger<SemanticDomainController> logger,
        ISemanticDomainService domainService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
    }

    /// <summary>
    /// Get all available semantic domain numbers.
    /// Example: GET /api/semantic-domain/domains
    /// </summary>
    /// <returns>List of available major domain numbers (1-93)</returns>
    [HttpGet("domains")]
    [ProducesResponseType(typeof(IReadOnlyList<int>), StatusCodes.Status200OK)]
    public IActionResult GetAvailableDomains()
    {
        _logger.LogInformation("Retrieving available semantic domains");
        var domains = _domainService.GetAvailableDomains();
        return Ok(domains);
    }

    /// <summary>
    /// Get all words in a semantic domain.
    /// Example: GET /api/semantic-domain/57?maxVersesPerWord=3
    /// </summary>
    /// <param name="majorDomain">The major domain number (e.g., 57 for "Possess, Transfer, Exchange")</param>
    /// <param name="maxVersesPerWord">Maximum sample verses to include per word (default: 3)</param>
    /// <returns>Domain search result with all word entries</returns>
    [HttpGet("{majorDomain:int}")]
    [ProducesResponseType(typeof(DomainSearchResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetWordsByDomain(int majorDomain, [FromQuery] int maxVersesPerWord = 3)
    {
        _logger.LogInformation(
            "Searching semantic domain {MajorDomain} with maxVersesPerWord={MaxVerses}",
            majorDomain, maxVersesPerWord);

        var result = _domainService.GetWordsByDomain(majorDomain, maxVersesPerWord);
        if (result is null)
        {
            _logger.LogWarning("Domain {MajorDomain} not found", majorDomain);
            return NotFound(new { Error = $"Semantic domain {majorDomain} not found" });
        }

        _logger.LogInformation(
            "Found {EntryCount} entries in domain {MajorDomain} ({Label})",
            result.TotalEntries, majorDomain, result.DomainLabel);

        return Ok(result);
    }

    /// <summary>
    /// Get words related to a specific Louw-Nida number within the same semantic domain.
    /// Example: GET /api/semantic-domain/related?louwNida=57.235&amp;limit=10
    /// </summary>
    /// <param name="louwNida">The Louw-Nida reference number (e.g., "57.235")</param>
    /// <param name="limit">Maximum related words to return (default: 10)</param>
    /// <param name="maxVersesPerWord">Maximum sample verses per word (default: 3)</param>
    /// <returns>Related words from the same semantic domain</returns>
    [HttpGet("related")]
    [ProducesResponseType(typeof(RelatedWordsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetRelatedWords(
        [FromQuery] string? louwNida,
        [FromQuery] int limit = 10,
        [FromQuery] int maxVersesPerWord = 3)
    {
        if (string.IsNullOrWhiteSpace(louwNida))
        {
            return BadRequest(new { Error = "louwNida parameter is required" });
        }

        _logger.LogInformation(
            "Finding related words for L-N {LouwNida} with limit={Limit}, maxVersesPerWord={MaxVerses}",
            louwNida, limit, maxVersesPerWord);

        var result = _domainService.GetRelatedWords(louwNida, limit, maxVersesPerWord);
        if (result is null)
        {
            _logger.LogWarning("Louw-Nida number {LouwNida} not found", louwNida);
            return NotFound(new { Error = $"Louw-Nida number '{louwNida}' not found" });
        }

        _logger.LogInformation(
            "Found {RelatedCount} related words for L-N {LouwNida} in domain {Domain} ({Label})",
            result.RelatedWords.Count, louwNida, result.MajorDomain, result.DomainLabel);

        return Ok(result);
    }
}
