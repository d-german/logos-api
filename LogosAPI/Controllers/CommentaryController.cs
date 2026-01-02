using LogosAPI.Models;
using LogosAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogosAPI.Controllers;

/// <summary>
/// Controller for Bible commentary endpoints using HelloAO API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class CommentaryController : ControllerBase
{
    private readonly ICommentaryService _commentaryService;
    private readonly IVerseReferenceNormalizer _normalizer;
    private readonly ILogger<CommentaryController> _logger;

    public CommentaryController(
        ICommentaryService commentaryService,
        IVerseReferenceNormalizer normalizer,
        ILogger<CommentaryController> logger)
    {
        _commentaryService = commentaryService ?? throw new ArgumentNullException(nameof(commentaryService));
        _normalizer = normalizer ?? throw new ArgumentNullException(nameof(normalizer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the list of available commentaries
    /// </summary>
    /// <returns>List of available commentaries with metadata</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(AvailableCommentariesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AvailableCommentariesResponse>> GetAvailableCommentaries(
        CancellationToken cancellationToken)
    {
        var result = await _commentaryService.GetAvailableCommentariesAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets commentary for a specific verse
    /// </summary>
    /// <param name="commentaryId">Commentary ID (e.g., "tyndale", "john-gill", "matthew-henry")</param>
    /// <param name="reference">Verse reference (e.g., "John.1.3", "Jn 1:3", "Gen.1.1")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Commentary for the specified verse</returns>
    [HttpGet("{commentaryId}/{reference}")]
    [ProducesResponseType(typeof(CommentaryLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentaryLookupResponse>> GetCommentary(
        string commentaryId,
        string reference,
        CancellationToken cancellationToken)
    {
        // Normalize the verse reference
        if (!_normalizer.TryNormalize(reference, out var normalizedRef))
        {
            _logger.LogWarning("Invalid verse reference: {Reference}", reference);
            return BadRequest(new { error = $"Invalid verse reference format: '{reference}'" });
        }

        // Parse the normalized reference (format: Book.Chapter.Verse)
        var parts = normalizedRef!.Split('.');
        if (parts.Length < 2)
        {
            return BadRequest(new { error = $"Invalid verse reference format: '{reference}'" });
        }

        var book = ConvertToHelloAoBookId(parts[0]);
        var chapter = int.Parse(parts[1]);
        int? verse = parts.Length >= 3 ? int.Parse(parts[2]) : null;

        var result = await _commentaryService.GetCommentaryAsync(
            commentaryId, 
            book, 
            chapter, 
            verse, 
            cancellationToken);

        if (result is null)
        {
            return NotFound(new { error = $"Commentary not found for '{reference}' in '{commentaryId}'" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets commentary from ALL available sources for one or more verses
    /// </summary>
    /// <param name="verseReferences">One or more verse references (e.g., "John.1.3", "Rom.8.28")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Commentary from all sources for the specified verses</returns>
    [HttpGet("all")]
    [ProducesResponseType(typeof(AllCommentariesLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AllCommentariesLookupResponse>> GetAllCommentaries(
        [FromQuery] string[] verseReferences,
        CancellationToken cancellationToken)
    {
        if (verseReferences is null || verseReferences.Length == 0)
        {
            return BadRequest(new { error = "At least one verse reference is required" });
        }

        var parsedReferences = new List<(string Book, int Chapter, int Verse, string OriginalRef)>();
        var invalidReferences = new List<string>();

        foreach (var reference in verseReferences)
        {
            if (!_normalizer.TryNormalize(reference, out var normalizedRef))
            {
                invalidReferences.Add(reference);
                continue;
            }

            var parts = normalizedRef!.Split('.');
            if (parts.Length < 3)
            {
                invalidReferences.Add(reference);
                continue;
            }

            var book = ConvertToHelloAoBookId(parts[0]);
            var chapter = int.Parse(parts[1]);
            var verse = int.Parse(parts[2]);

            parsedReferences.Add((book, chapter, verse, normalizedRef));
        }

        if (invalidReferences.Count > 0)
        {
            _logger.LogWarning("Invalid verse references: {References}", string.Join(", ", invalidReferences));
        }

        if (parsedReferences.Count == 0)
        {
            return BadRequest(new { error = $"No valid verse references provided. Invalid: {string.Join(", ", invalidReferences)}" });
        }

        var result = await _commentaryService.GetAllCommentariesAsync(parsedReferences, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Converts internal book ID to HelloAO format
    /// </summary>
    private static string ConvertToHelloAoBookId(string internalBookId)
    {
        // Map common book abbreviations to HelloAO book IDs (USFM format)
        return internalBookId.ToUpperInvariant() switch
        {
            "MATT" => "MAT",
            "MARK" => "MRK",
            "LUKE" => "LUK",
            "JOHN" => "JHN",
            "ACTS" => "ACT",
            "ROM" => "ROM",
            "1COR" => "1CO",
            "2COR" => "2CO",
            "GAL" => "GAL",
            "EPH" => "EPH",
            "PHIL" => "PHP",
            "COL" => "COL",
            "1THESS" => "1TH",
            "2THESS" => "2TH",
            "1TIM" => "1TI",
            "2TIM" => "2TI",
            "TITUS" => "TIT",
            "PHLM" => "PHM",
            "HEB" => "HEB",
            "JAS" => "JAS",
            "1PET" => "1PE",
            "2PET" => "2PE",
            "1JOHN" => "1JN",
            "2JOHN" => "2JN",
            "3JOHN" => "3JN",
            "JUDE" => "JUD",
            "REV" => "REV",
            // Old Testament
            "GEN" => "GEN",
            "EXOD" => "EXO",
            "LEV" => "LEV",
            "NUM" => "NUM",
            "DEUT" => "DEU",
            "JOSH" => "JOS",
            "JUDG" => "JDG",
            "RUTH" => "RUT",
            "1SAM" => "1SA",
            "2SAM" => "2SA",
            "1KGS" => "1KI",
            "2KGS" => "2KI",
            "1CHR" => "1CH",
            "2CHR" => "2CH",
            "EZRA" => "EZR",
            "NEH" => "NEH",
            "ESTH" => "EST",
            "JOB" => "JOB",
            "PS" => "PSA",
            "PROV" => "PRO",
            "ECCL" => "ECC",
            "SONG" => "SNG",
            "ISA" => "ISA",
            "JER" => "JER",
            "LAM" => "LAM",
            "EZEK" => "EZK",
            "DAN" => "DAN",
            "HOS" => "HOS",
            "JOEL" => "JOL",
            "AMOS" => "AMO",
            "OBAD" => "OBA",
            "JONAH" => "JON",
            "MIC" => "MIC",
            "NAH" => "NAM",
            "HAB" => "HAB",
            "ZEPH" => "ZEP",
            "HAG" => "HAG",
            "ZECH" => "ZEC",
            "MAL" => "MAL",
            _ => internalBookId.ToUpperInvariant()
        };
    }
}
