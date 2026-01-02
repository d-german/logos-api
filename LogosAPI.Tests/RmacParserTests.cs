using LogosAPI.Models;
using LogosAPI.Services;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for RmacParser
/// </summary>
public sealed class RmacParserTests
{
    private readonly IRmacParser _parser = new RmacParser();

    #region Finite Verb Tests

    [Theory]
    [InlineData("V-AAI-3S", "Verb", "Aorist", "Active", "Finite", "Indicative", "Third", "Singular")]
    [InlineData("V-PAI-1P", "Verb", "Present", "Active", "Finite", "Indicative", "First", "Plural")]
    [InlineData("V-RAI-1P", "Verb", "Perfect", "Active", "Finite", "Indicative", "First", "Plural")]
    [InlineData("V-FMI-2S", "Verb", "Future", "Middle", "Finite", "Indicative", "Second", "Singular")]
    [InlineData("V-IAI-3P", "Verb", "Imperfect", "Active", "Finite", "Indicative", "Third", "Plural")]
    [InlineData("V-LAI-1S", "Verb", "Pluperfect", "Active", "Finite", "Indicative", "First", "Singular")]
    public void Parse_FiniteVerb_ReturnsCorrectComponents(
        string code, string pos, string tense, string voice, string verbForm, string mood, string person, string number)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(verbForm, result.VerbForm);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(person, result.Person);
        Assert.Equal(number, result.Number);
    }

    [Theory]
    [InlineData("V-2AAI-3S", "Verb", "SecondAorist", "Active", "Finite", "Indicative", "Third", "Singular")]
    [InlineData("V-2RAI-1P", "Verb", "SecondPerfect", "Active", "Finite", "Indicative", "First", "Plural")]
    public void Parse_SecondaryTenseVerb_ReturnsCorrectTense(
        string code, string pos, string tense, string voice, string verbForm, string mood, string person, string number)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(verbForm, result.VerbForm);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(person, result.Person);
        Assert.Equal(number, result.Number);
    }

    [Theory]
    [InlineData("V-PAS-3S", "Verb", "Present", "Active", "Finite", "Subjunctive", "Third", "Singular")]
    [InlineData("V-AAM-2P", "Verb", "Aorist", "Active", "Finite", "Imperative", "Second", "Plural")]
    [InlineData("V-PAO-3S", "Verb", "Present", "Active", "Finite", "Optative", "Third", "Singular")]
    public void Parse_SubjunctiveImperativeOptative_ReturnsCorrectMood(
        string code, string pos, string tense, string voice, string verbForm, string mood, string person, string number)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(verbForm, result.VerbForm);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(person, result.Person);
        Assert.Equal(number, result.Number);
    }

    #endregion

    #region Infinitive Tests

    [Theory]
    [InlineData("V-AAN", "Verb", "Aorist", "Active", "Infinitive")]
    [InlineData("V-PAN", "Verb", "Present", "Active", "Infinitive")]
    [InlineData("V-PMN", "Verb", "Present", "Middle", "Infinitive")]
    public void Parse_Infinitive_ReturnsCorrectComponents(
        string code, string pos, string tense, string voice, string verbForm)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(verbForm, result.VerbForm);
        Assert.Null(result.Mood);
        Assert.Null(result.Person);
        Assert.Null(result.Number);
    }

    #endregion

    #region Participle Tests

    [Theory]
    [InlineData("V-PAP-NSM", "Verb", "Present", "Active", "Participle", "Nominative", "Singular", "Masculine")]
    [InlineData("V-AAP-GPF", "Verb", "Aorist", "Active", "Participle", "Genitive", "Plural", "Feminine")]
    [InlineData("V-PMP-DPN", "Verb", "Present", "Middle", "Participle", "Dative", "Plural", "Neuter")]
    public void Parse_Participle_ReturnsCorrectComponents(
        string code, string pos, string tense, string voice, string verbForm, 
        string grammaticalCase, string number, string gender)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(verbForm, result.VerbForm);
        Assert.Null(result.Mood); // Participles have no mood
        Assert.Equal(grammaticalCase, result.Case);
        Assert.Equal(number, result.Number);
        Assert.Equal(gender, result.Gender);
        Assert.Null(result.Person); // Participles have no person
    }

    #endregion

    #region Voice Variation Tests

    [Theory]
    [InlineData("V-PPI-3S", "Passive", false)]
    [InlineData("V-PMI-3S", "Middle", false)]
    [InlineData("V-PEI-3S", "MiddleOrPassive", false)]
    [InlineData("V-PDI-3S", "Deponent", true)]
    [InlineData("V-PNI-3S", "MiddleOrPassiveDeponent", true)]
    [InlineData("V-POI-3S", "PassiveDeponent", true)]
    public void Parse_VerbVoiceVariations_ReturnsCorrectVoiceAndDeponentFlag(string code, string expectedVoice, bool hasDeponentFlag)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(expectedVoice, result.Voice);
        
        if (hasDeponentFlag)
            Assert.Contains("Deponent", result.Flags);
        else
            Assert.DoesNotContain("Deponent", result.Flags);
    }

    #endregion

    #region Personal Pronoun Tests

    [Fact]
    public void Parse_PersonalPronoun_FirstPersonNominativeSingular()
    {
        // P-1NS = Personal pronoun, first person, Nominative, Singular
        var result = _parser.Parse("P-1NS");

        Assert.NotNull(result);
        Assert.Equal("PersonalPronoun", result.Pos);
        Assert.Equal("First", result.Person);
        Assert.Equal("Nominative", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Null(result.Gender); // 1st/2nd person don't have gender
    }

    [Fact]
    public void Parse_PersonalPronoun_SecondPersonGenitivePlural()
    {
        // P-2GP = Personal pronoun, second person, Genitive, Plural
        var result = _parser.Parse("P-2GP");

        Assert.NotNull(result);
        Assert.Equal("PersonalPronoun", result.Pos);
        Assert.Equal("Second", result.Person);
        Assert.Equal("Genitive", result.Case);
        Assert.Equal("Plural", result.Number);
        Assert.Null(result.Gender);
    }

    [Fact]
    public void Parse_PersonalPronoun_ThirdPersonWithGender()
    {
        // P-ASM = Personal pronoun, (3rd person implied), Accusative, Singular, Masculine
        var result = _parser.Parse("P-ASM");

        Assert.NotNull(result);
        Assert.Equal("PersonalPronoun", result.Pos);
        Assert.Equal("Third", result.Person);
        Assert.Equal("Accusative", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Equal("Masculine", result.Gender);
    }

    #endregion

    #region Noun Tests

    [Theory]
    [InlineData("N-NSM", "Noun", "Nominative", "Singular", "Masculine")]
    [InlineData("N-GSF", "Noun", "Genitive", "Singular", "Feminine")]
    [InlineData("N-DPN", "Noun", "Dative", "Plural", "Neuter")]
    [InlineData("N-APM", "Noun", "Accusative", "Plural", "Masculine")]
    [InlineData("N-VSF", "Noun", "Vocative", "Singular", "Feminine")]
    public void Parse_Noun_ReturnsCorrectComponents(
        string code, string pos, string grammaticalCase, string number, string gender)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(grammaticalCase, result.Case);
        Assert.Equal(number, result.Number);
        Assert.Equal(gender, result.Gender);
    }

    [Theory]
    [InlineData("N-GSM-P", "ProperName")]
    [InlineData("N-NSM-T", "Title")]
    [InlineData("N-ASF-L", "Location")]
    public void Parse_NounWithFlag_ReturnsCorrectFlag(string code, string expectedFlag)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal("Noun", result.Pos);
        Assert.Contains(expectedFlag, result.Flags);
    }

    #endregion

    #region Adjective Tests

    [Theory]
    [InlineData("A-NSM", "Adjective", "Nominative", "Singular", "Masculine")]
    [InlineData("A-GPF", "Adjective", "Genitive", "Plural", "Feminine")]
    public void Parse_Adjective_ReturnsCorrectComponents(
        string code, string pos, string grammaticalCase, string number, string gender)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(grammaticalCase, result.Case);
        Assert.Equal(number, result.Number);
        Assert.Equal(gender, result.Gender);
    }

    [Theory]
    [InlineData("A-NSM-C", "Comparative")]
    [InlineData("A-NSM-S", "Superlative")]
    public void Parse_AdjectiveWithFlag_ReturnsCorrectFlag(string code, string expectedFlag)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal("Adjective", result.Pos);
        Assert.Contains(expectedFlag, result.Flags);
    }

    #endregion

    #region Article Tests

    [Theory]
    [InlineData("T-NSM", "Article", "Nominative", "Singular", "Masculine")]
    [InlineData("T-ASF", "Article", "Accusative", "Singular", "Feminine")]
    [InlineData("T-GPN", "Article", "Genitive", "Plural", "Neuter")]
    public void Parse_Article_ReturnsCorrectComponents(
        string code, string pos, string grammaticalCase, string number, string gender)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(grammaticalCase, result.Case);
        Assert.Equal(number, result.Number);
        Assert.Equal(gender, result.Gender);
    }

    #endregion

    #region Other Pronoun Tests

    [Theory]
    [InlineData("D-NSM", "DemonstrativePronoun")]
    [InlineData("R-ASF", "RelativePronoun")]
    [InlineData("I-NSN", "InterrogativePronoun")]
    [InlineData("X-GSM", "IndefinitePronoun")]
    [InlineData("K-APM", "CorrelativePronoun")]
    [InlineData("C-DPM", "ReciprocalPronoun")]
    public void Parse_OtherPronoun_ReturnsCorrectType(string code, string expectedPos)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(expectedPos, result.Pos);
    }

    #endregion

    #region Simple Types Tests

    [Theory]
    [InlineData("CONJ", "Conjunction")]
    [InlineData("INJ", "Interjection")]
    [InlineData("PREP", "Preposition")]
    [InlineData("PRT", "Particle")]
    [InlineData("ARAM", "Aramaic")]
    [InlineData("HEB", "Hebrew")]
    public void Parse_SimpleType_ReturnsCorrectPos(string code, string expectedPos)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(expectedPos, result.Pos);
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_ConjunctionWithTitle_ReturnsFlag()
    {
        var result = _parser.Parse("CONJ-T");

        Assert.NotNull(result);
        Assert.Equal("Conjunction", result.Pos);
        Assert.Contains("Title", result.Flags);
    }

    #endregion

    #region Adverb Tests

    [Fact]
    public void Parse_Adverb_ReturnsAdverb()
    {
        var result = _parser.Parse("ADV");

        Assert.NotNull(result);
        Assert.Equal("Adverb", result.Pos);
        Assert.Empty(result.Flags);
    }

    [Theory]
    [InlineData("ADV-C", "Comparative")]
    [InlineData("ADV-S", "Superlative")]
    [InlineData("ADV-I", "Interrogative")]
    [InlineData("ADV-N", "Negative")]
    public void Parse_AdverbWithFlag_ReturnsCorrectFlag(string code, string expectedFlag)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal("Adverb", result.Pos);
        Assert.Contains(expectedFlag, result.Flags);
    }

    #endregion

    #region Edge Cases and Invalid Input Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_NullOrEmpty_ReturnsNull(string? code)
    {
        var result = _parser.Parse(code);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_ValidCode_ReturnsTrue()
    {
        var success = _parser.TryParse("V-AAI-3S", out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("Verb", result.Pos);
    }

    [Fact]
    public void TryParse_InvalidCode_ReturnsFalse()
    {
        var success = _parser.TryParse("INVALID", out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void Parse_CaseInsensitive_ReturnsParsedResult()
    {
        var result = _parser.Parse("v-aai-3s");

        Assert.NotNull(result);
        Assert.Equal("Verb", result.Pos);
        Assert.Equal("Aorist", result.Tense);
    }

    #endregion

    #region Real-World Examples

    [Fact]
    public void Parse_RealExample_PersonalPronounFirstNominativeSingular()
    {
        // ἐγὼ (I) from various NT passages
        var result = _parser.Parse("P-1NS");

        Assert.NotNull(result);
        Assert.Equal("PersonalPronoun", result.Pos);
        Assert.Equal("First", result.Person);
        Assert.Equal("Nominative", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Null(result.Gender);
        Assert.Null(result.Tense);
        Assert.Null(result.Voice);
        Assert.Null(result.VerbForm);
        Assert.Null(result.Mood);
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_RealExample_VerbAoristActiveIndicativeThirdSingular()
    {
        // ἐγέννησεν (begat) from Matt.1.2
        var result = _parser.Parse("V-AAI-3S");

        Assert.NotNull(result);
        Assert.Equal("Verb", result.Pos);
        Assert.Equal("Aorist", result.Tense);
        Assert.Equal("Active", result.Voice);
        Assert.Equal("Finite", result.VerbForm);
        Assert.Equal("Indicative", result.Mood);
        Assert.Equal("Third", result.Person);
        Assert.Equal("Singular", result.Number);
        Assert.Null(result.Case);
        Assert.Null(result.Gender);
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_RealExample_Participle()
    {
        // ἀγαπῶσιν (loving) from Rom 8:28
        var result = _parser.Parse("V-PAP-DPM");

        Assert.NotNull(result);
        Assert.Equal("Verb", result.Pos);
        Assert.Equal("Present", result.Tense);
        Assert.Equal("Active", result.Voice);
        Assert.Equal("Participle", result.VerbForm);
        Assert.Null(result.Mood); // Participles don't have mood
        Assert.Equal("Dative", result.Case);
        Assert.Equal("Plural", result.Number);
        Assert.Equal("Masculine", result.Gender);
        Assert.Null(result.Person); // Participles don't have person
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_RealExample_NounWithProperNameFlag()
    {
        // Ἰησοῦ (Jesus) from Matt.1.1
        var result = _parser.Parse("N-GSM-P");

        Assert.NotNull(result);
        Assert.Equal("Noun", result.Pos);
        Assert.Equal("Genitive", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Equal("Masculine", result.Gender);
        Assert.Contains("ProperName", result.Flags);
    }

    [Fact]
    public void Parse_RealExample_Article()
    {
        // τὸν (the) from Matt.1.2
        var result = _parser.Parse("T-ASM");

        Assert.NotNull(result);
        Assert.Equal("Article", result.Pos);
        Assert.Equal("Accusative", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Equal("Masculine", result.Gender);
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_RealExample_Conjunction()
    {
        // δὲ (and/but) from Matt.1.2
        var result = _parser.Parse("CONJ");

        Assert.NotNull(result);
        Assert.Equal("Conjunction", result.Pos);
        Assert.Null(result.Case);
        Assert.Null(result.Number);
        Assert.Null(result.Gender);
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_RealExample_SecondAoristFiniteVerb()
    {
        // Εἶπεν (said) from John 8:58
        var result = _parser.Parse("V-2AAI-3S");

        Assert.NotNull(result);
        Assert.Equal("Verb", result.Pos);
        Assert.Equal("SecondAorist", result.Tense);
        Assert.Equal("Active", result.Voice);
        Assert.Equal("Finite", result.VerbForm);
        Assert.Equal("Indicative", result.Mood);
        Assert.Equal("Third", result.Person);
        Assert.Equal("Singular", result.Number);
    }

    #endregion

    #region Flags Collection Tests

    [Fact]
    public void Parse_FlagsIsAlwaysInitialized()
    {
        var result = _parser.Parse("N-NSM");

        Assert.NotNull(result);
        Assert.NotNull(result.Flags);
        Assert.Empty(result.Flags);
    }

    [Fact]
    public void Parse_DeponentVerbHasDeponentFlag()
    {
        var result = _parser.Parse("V-ADI-3S");

        Assert.NotNull(result);
        Assert.Equal("Deponent", result.Voice);
        Assert.Contains("Deponent", result.Flags);
    }

    #endregion
}
