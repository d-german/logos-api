namespace LogosAPI.Models;

/// <summary>
/// Represents available commentaries from HelloAO API
/// </summary>
public sealed record CommentaryInfo(
    string Id,
    string Name,
    string EnglishName,
    string Website,
    string LicenseUrl,
    string? LicenseNotes,
    int NumberOfBooks,
    int TotalNumberOfChapters,
    int TotalNumberOfVerses
);

/// <summary>
/// Response for listing available commentaries
/// </summary>
public sealed record AvailableCommentariesResponse(
    IReadOnlyList<CommentaryInfo> Commentaries
);

/// <summary>
/// Response for a verse commentary lookup
/// </summary>
public sealed record CommentaryLookupResponse(
    string Reference,
    string CommentaryId,
    string CommentaryName,
    IReadOnlyList<VerseCommentary> Verses,
    IReadOnlyList<string> NotFound
);

/// <summary>
/// Commentary content for a single verse
/// </summary>
public sealed record VerseCommentary(
    int VerseNumber,
    string Content
);
