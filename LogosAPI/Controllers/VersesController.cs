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
    /// Look up Bible verses with lexicon data
    /// Example: GET /api/verses/lookup?verseReferences=Matt.1.1&amp;verseReferences=John.3.16
    /// </summary>
    [HttpGet("lookup")]
    public IActionResult LookupVerses([FromQuery] string[] verseReferences)
    {
        _logger.LogInformation(
            "Received verse lookup request. References: {References}",
            string.Join(", ", verseReferences));

        var result = _verseLookupService.LookupVerses(verseReferences);

        _logger.LogInformation(
            "Returning response with {FoundCount} verses found, {NotFoundCount} not found",
            result.Verses.Count,
            result.NotFound.Count);

        return Ok(result);
    }

    /// <summary>
    /// Health check endpoint with service status
    /// </summary>
    [HttpGet("_health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Initialized = _bibleDataService.IsInitialized,
            VersesCount = _bibleDataService.VersesCount,
            LexiconCount = _bibleDataService.LexiconCount
        });
    }
}
