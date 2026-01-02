using LogosAPI.Models;
using LogosAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogosAPI.Controllers;

/// <summary>
/// Controller for lexicon lookup operations
/// Single Responsibility: Handle HTTP requests for lexicon data
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class LexiconController : ControllerBase
{
    private readonly ILogger<LexiconController> _logger;
    private readonly IBibleDataService _bibleDataService;
    private readonly IStrongsNumberNormalizer _strongsNormalizer;

    public LexiconController(
        ILogger<LexiconController> logger,
        IBibleDataService bibleDataService,
        IStrongsNumberNormalizer strongsNormalizer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bibleDataService = bibleDataService ?? throw new ArgumentNullException(nameof(bibleDataService));
        _strongsNormalizer = strongsNormalizer ?? throw new ArgumentNullException(nameof(strongsNormalizer));
    }

    /// <summary>
    /// Look up a lexicon entry by Strong's number
    /// Example: GET /api/lexicon/G25
    /// </summary>
    /// <param name="strongsNumber">The Strong's number (e.g., G25, H1234)</param>
    /// <returns>The lexicon definition for the Strong's number</returns>
    [HttpGet("{strongsNumber}")]
    public IActionResult GetLexiconEntry(string strongsNumber)
    {
        LogLookupRequest(strongsNumber);

        if (!_strongsNormalizer.TryNormalize(strongsNumber, out var normalized))
        {
            LogInvalidStrongs(strongsNumber);
            return BadRequest(new { Error = $"Invalid Strong's number format: '{strongsNumber}'" });
        }

        if (!_bibleDataService.Lexicon.TryGetValue(normalized!, out var definition))
        {
            LogNotFound(normalized!);
            return NotFound(new { Error = $"Lexicon entry not found for: '{normalized}'" });
        }

        LogFound(normalized!);
        return Ok(new LexiconLookupResponse(normalized!, definition));
    }

    /// <summary>
    /// Logs the incoming lookup request
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogLookupRequest(string strongsNumber)
    {
        _logger.LogInformation("Received lexicon lookup request for: {StrongsNumber}", strongsNumber);
    }

    /// <summary>
    /// Logs invalid Strong's number
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogInvalidStrongs(string strongsNumber)
    {
        _logger.LogWarning("Invalid Strong's number format: {StrongsNumber}", strongsNumber);
    }

    /// <summary>
    /// Logs not found Strong's number
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogNotFound(string normalized)
    {
        _logger.LogWarning("Lexicon entry not found for: {StrongsNumber}", normalized);
    }

    /// <summary>
    /// Logs successful lookup
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogFound(string normalized)
    {
        _logger.LogInformation("Returning lexicon entry for: {StrongsNumber}", normalized);
    }
}
