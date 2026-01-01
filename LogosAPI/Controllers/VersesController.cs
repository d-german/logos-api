using LogosAPI.Models;
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

    public VersesController(ILogger<VersesController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Look up Bible verses (POST)
    /// Cyclomatic Complexity: 2
    /// </summary>
    [HttpPost("lookup")]
    public IActionResult LookupVersesPost([FromBody] VerseLookupRequest request)
    {
        LogRequestReceived("POST", request.VerseReferences);

        var response = CreateResponse(request.VerseReferences);

        LogResponseReturned(response.VerseReferences.Length);

        return Ok(response);
    }

    /// <summary>
    /// Look up Bible verses (GET)
    /// Cyclomatic Complexity: 2
    /// </summary>
    [HttpGet("lookup")]
    public IActionResult LookupVersesGet([FromQuery] string[] verseReferences)
    {
        LogRequestReceived("GET", verseReferences);

        var response = CreateResponse(verseReferences);

        LogResponseReturned(response.VerseReferences.Length);

        return Ok(response);
    }

    /// <summary>
    /// Create response object
    /// Cyclomatic Complexity: 1
    /// </summary>
    private static VerseLookupResponse CreateResponse(string[] verseReferences)
    {
        return new VerseLookupResponse
        {
            Message = "Hello World",
            VerseReferences = verseReferences
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
    private void LogResponseReturned(int count)
    {
        _logger.LogInformation(
            "Returning response with {Count} verse references",
            count);
    }
}
