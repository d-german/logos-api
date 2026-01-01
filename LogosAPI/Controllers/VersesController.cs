using LogosAPI.Models;
using LogosAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogosAPI.Controllers;

/// <summary>
/// Controller for Bible verse lookup operations
/// Single Responsibility: Handle HTTP requests for verse data
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class VersesController : ControllerBase
{
    private readonly ILogger<VersesController> _logger;
    private readonly IBibleDataService _bibleDataService;
    private readonly IVerseLookupService _verseLookupService;

    public VersesController(
        ILogger<VersesController> logger,
        IBibleDataService bibleDataService,
        IVerseLookupService verseLookupService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _bibleDataService = bibleDataService ?? throw new ArgumentNullException(nameof(bibleDataService));
        _verseLookupService = verseLookupService ?? throw new ArgumentNullException(nameof(verseLookupService));
    }

    /// <summary>
    /// Look up Bible verses with lexicon data (POST)
    /// Cyclomatic Complexity: 1
    /// </summary>
    [HttpPost("lookup")]
    public IActionResult LookupVersesPost([FromBody] VerseLookupRequest request)
    {
        LogRequestReceived("POST", request.VerseReferences);

        var result = _verseLookupService.LookupVerses(request.VerseReferences);

        LogResponseReturned(result.Verses.Count, result.NotFound.Count);

        return Ok(result);
    }

    /// <summary>
    /// Look up Bible verses with lexicon data (GET)
    /// Cyclomatic Complexity: 1
    /// </summary>
    [HttpGet("lookup")]
    public IActionResult LookupVersesGet([FromQuery] string[] verseReferences)
    {
        LogRequestReceived("GET", verseReferences);

        var result = _verseLookupService.LookupVerses(verseReferences);

        LogResponseReturned(result.Verses.Count, result.NotFound.Count);

        return Ok(result);
    }

    /// <summary>
    /// Health check endpoint with service status
    /// Cyclomatic Complexity: 1
    /// </summary>
    [HttpGet("_health")]
    public IActionResult Health()
    {
        _logger.LogDebug("Health check requested");
        return Ok(CreateHealthResponse());
    }

    /// <summary>
    /// Creates health response object
    /// Cyclomatic Complexity: 1
    /// </summary>
    private object CreateHealthResponse()
    {
        return new
        {
            Status = "Healthy",
            Initialized = _bibleDataService.IsInitialized,
            VersesCount = _bibleDataService.VersesCount,
            LexiconCount = _bibleDataService.LexiconCount
        };
    }

    /// <summary>
    /// Log incoming request
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogRequestReceived(string method, string[] verseReferences)
    {
        _logger.LogInformation(
            "Received {Method} request for verse lookup. References: {References}",
            method,
            string.Join(", ", verseReferences));
    }

    /// <summary>
    /// Log outgoing response
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogResponseReturned(int foundCount, int notFoundCount)
    {
        _logger.LogInformation(
            "Returning response with {FoundCount} verses found, {NotFoundCount} not found",
            foundCount,
            notFoundCount);
    }
}
