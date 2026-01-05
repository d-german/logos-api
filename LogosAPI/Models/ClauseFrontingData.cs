namespace LogosAPI.Models;

/// <summary>
/// Represents clause-level fronting data indicating marked word order for emphasis
/// </summary>
/// <param name="ClauseNumber">The clause number within the verse (1-based)</param>
/// <param name="Pattern">The constituent order pattern (e.g., P-VC-S for fronted predicate)</param>
/// <param name="FrontedElement">The Greek word or phrase that is fronted</param>
/// <param name="FrontedRole">The grammatical role of the fronted element (e.g., predicate, object)</param>
/// <param name="GreekText">The full Greek text of the clause (optional)</param>
/// <param name="FrontedElementGloss">The English gloss/translation of the fronted element (optional)</param>
public sealed record ClauseFrontingData(
    int ClauseNumber,
    string Pattern,
    string FrontedElement,
    string FrontedRole,
    string? GreekText,
    string? FrontedElementGloss = null
);
