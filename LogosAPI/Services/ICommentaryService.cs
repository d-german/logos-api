using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service interface for retrieving Bible commentary from HelloAO API
/// </summary>
public interface ICommentaryService
{
    /// <summary>
    /// Gets the list of available commentaries
    /// </summary>
    Task<AvailableCommentariesResponse> GetAvailableCommentariesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets commentary for a specific verse or chapter
    /// </summary>
    /// <param name="commentaryId">The commentary ID (e.g., "tyndale", "john-gill")</param>
    /// <param name="book">The book ID (e.g., "JHN", "GEN")</param>
    /// <param name="chapter">The chapter number</param>
    /// <param name="verse">Optional verse number. If null, returns entire chapter commentary</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<CommentaryLookupResponse?> GetCommentaryAsync(
        string commentaryId, 
        string book, 
        int chapter, 
        int? verse = null,
        CancellationToken cancellationToken = default);
}
