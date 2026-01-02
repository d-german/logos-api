using LogosAPI.Models;
using LogosAPI.Services;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for RmacParser
/// </summary>
public sealed class RmacParserTests
{
    private readonly IRmacParser _parser = new RmacParser();

    #region Verb Tests

    [Theory]
    [InlineData("V-AAI-3S", "Verb", "Aorist", "Active", "Indicative", "Third", "Singular")]
    [InlineData("V-PAI-1P", "Verb", "Present", "Active", "Indicative", "First", "Plural")]
    [InlineData("V-RAI-1P", "Verb", "Perfect", "Active", "Indicative", "First", "Plural")]
    [InlineData("V-FMI-2S", "Verb", "Future", "Middle", "Indicative", "Second", "Singular")]
    [InlineData("V-IAI-3P", "Verb", "Imperfect", "Active", "Indicative", "Third", "Plural")]
    [InlineData("V-LAI-1S", "Verb", "Pluperfect", "Active", "Indicative", "First", "Singular")]
    public void Parse_FiniteVerb_ReturnsCorrectComponents(
        string code, string pos, string tense, string voice, string mood, string person, string number)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(person, result.Person);
        Assert.Equal(number, result.Number);
    }

    [Theory]
    [InlineData("V-2AAI-3S", "Verb", "SecondAorist", "Active", "Indicative", "Third", "Singular")]
    [InlineData("V-2RAI-1P", "Verb", "SecondPerfect", "Active", "Indicative", "First", "Plural")]
    public void Parse_SecondaryTenseVerb_ReturnsCorrectTense(
        string code, string pos, string tense, string voice, string mood, string person, string number)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(person, result.Person);
        Assert.Equal(number, result.Number);
    }

    [Theory]
    [InlineData("V-AAN", "Verb", "Aorist", "Active", "Infinitive")]
    [InlineData("V-PAN", "Verb", "Present", "Active", "Infinitive")]
    [InlineData("V-PMN", "Verb", "Present", "Middle", "Infinitive")]
    public void Parse_Infinitive_ReturnsCorrectComponents(
        string code, string pos, string tense, string voice, string mood)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(mood, result.Mood);
        Assert.Null(result.Person);
        Assert.Null(result.Number);
    }

    [Theory]
    [InlineData("V-PAP-NSM", "Verb", "Present", "Active", "Participle", "Nominative", "Singular", "Masculine")]
    [InlineData("V-AAP-GPF", "Verb", "Aorist", "Active", "Participle", "Genitive", "Plural", "Feminine")]
    [InlineData("V-PMP-DPN", "Verb", "Present", "Middle", "Participle", "Dative", "Plural", "Neuter")]
    public void Parse_Participle_ReturnsCorrectComponents(
        string code, string pos, string tense, string voice, string mood, 
        string grammaticalCase, string number, string gender)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(grammaticalCase, result.Case);
        Assert.Equal(number, result.Number);
        Assert.Equal(gender, result.Gender);
    }

    [Theory]
    [InlineData("V-PAS-3S", "Verb", "Present", "Active", "Subjunctive", "Third", "Singular")]
    [InlineData("V-AAM-2P", "Verb", "Aorist", "Active", "Imperative", "Second", "Plural")]
    [InlineData("V-PAO-3S", "Verb", "Present", "Active", "Optative", "Third", "Singular")]
    public void Parse_SubjunctiveImperativeOptative_ReturnsCorrectMood(
        string code, string pos, string tense, string voice, string mood, string person, string number)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(pos, result.Pos);
        Assert.Equal(tense, result.Tense);
        Assert.Equal(voice, result.Voice);
        Assert.Equal(mood, result.Mood);
        Assert.Equal(person, result.Person);
        Assert.Equal(number, result.Number);
    }

    [Theory]
    [InlineData("V-PPI-3S", "Passive")]
    [InlineData("V-PMI-3S", "Middle")]
    [InlineData("V-PEI-3S", "MiddleOrPassive")]
    [InlineData("V-PDI-3S", "Deponent")]
    public void Parse_VerbVoiceVariations_ReturnsCorrectVoice(string code, string expectedVoice)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(expectedVoice, result.Voice);
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
    [InlineData("N-GSM-P", "Person")]
    [InlineData("N-NSM-T", "Title")]
    [InlineData("N-ASF-L", "Location")]
    public void Parse_NounWithSuffix_ReturnsCorrectSuffix(string code, string expectedSuffix)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal("Noun", result.Pos);
        Assert.Equal(expectedSuffix, result.Suffix);
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
    public void Parse_AdjectiveWithSuffix_ReturnsCorrectSuffix(string code, string expectedSuffix)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal("Adjective", result.Pos);
        Assert.Equal(expectedSuffix, result.Suffix);
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

    #region Pronoun Tests

    [Theory]
    [InlineData("D-NSM", "DemonstrativePronoun")]
    [InlineData("R-ASF", "RelativePronoun")]
    [InlineData("I-NSN", "InterrogativePronoun")]
    [InlineData("X-GSM", "IndefinitePronoun")]
    [InlineData("K-APM", "CorrelativePronoun")]
    [InlineData("C-DPM", "ReciprocalPronoun")]
    public void Parse_Pronoun_ReturnsCorrectType(string code, string expectedPos)
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
    }

    [Theory]
    [InlineData("CONJ-T", "Conjunction", "Title")]
    [InlineData("PREP-G", "Preposition", "Genitive")]
    [InlineData("PREP-D", "Preposition", "Dative")]
    [InlineData("PREP-A", "Preposition", "Accusative")]
    public void Parse_SimpleTypeWithSuffix_ReturnsCorrectSuffix(string code, string expectedPos, string expectedSuffix)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal(expectedPos, result.Pos);
        Assert.Equal(expectedSuffix, result.Suffix);
    }

    #endregion

    #region Adverb Tests

    [Fact]
    public void Parse_Adverb_ReturnsAdverb()
    {
        var result = _parser.Parse("ADV");

        Assert.NotNull(result);
        Assert.Equal("Adverb", result.Pos);
    }

    [Theory]
    [InlineData("ADV-C", "Comparative")]
    [InlineData("ADV-S", "Superlative")]
    [InlineData("ADV-I", "Interrogative")]
    [InlineData("ADV-N", "Negative")]
    public void Parse_AdverbWithSuffix_ReturnsCorrectSuffix(string code, string expectedSuffix)
    {
        var result = _parser.Parse(code);

        Assert.NotNull(result);
        Assert.Equal("Adverb", result.Pos);
        Assert.Equal(expectedSuffix, result.Suffix);
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

    #region Real-World Examples from verses.json

    [Fact]
    public void Parse_RealExample_NounGenitiveSingularMasculinePerson()
    {
        // From Matt.1.1: "N-GSM-P" -> "Noun, Genitive, Singular, Masculine, Person"
        var result = _parser.Parse("N-GSM-P");

        Assert.NotNull(result);
        Assert.Equal("Noun", result.Pos);
        Assert.Equal("Genitive", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Equal("Masculine", result.Gender);
        Assert.Equal("Person", result.Suffix);
    }

    [Fact]
    public void Parse_RealExample_VerbAoristActiveIndicativeThirdSingular()
    {
        // From Matt.1.2: "V-AAI-3S" -> "Verb, Aorist, Active, Indicative, third, Singular"
        var result = _parser.Parse("V-AAI-3S");

        Assert.NotNull(result);
        Assert.Equal("Verb", result.Pos);
        Assert.Equal("Aorist", result.Tense);
        Assert.Equal("Active", result.Voice);
        Assert.Equal("Indicative", result.Mood);
        Assert.Equal("Third", result.Person);
        Assert.Equal("Singular", result.Number);
    }

    [Fact]
    public void Parse_RealExample_ArticleAccusativeSingularMasculine()
    {
        // From Matt.1.2: "T-ASM" -> "definite article, Accusative, Singular, Masculine"
        var result = _parser.Parse("T-ASM");

        Assert.NotNull(result);
        Assert.Equal("Article", result.Pos);
        Assert.Equal("Accusative", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Equal("Masculine", result.Gender);
    }

    [Fact]
    public void Parse_RealExample_Conjunction()
    {
        // From Matt.1.2: "CONJ" -> "CONJunction or conjunctive particle"
        var result = _parser.Parse("CONJ");

        Assert.NotNull(result);
        Assert.Equal("Conjunction", result.Pos);
    }

    [Fact]
    public void Parse_RealExample_NounNominativeSingularFeminine()
    {
        // From Matt.1.1: "N-NSF" -> "Noun, Nominative, Singular, Feminine"
        var result = _parser.Parse("N-NSF");

        Assert.NotNull(result);
        Assert.Equal("Noun", result.Pos);
        Assert.Equal("Nominative", result.Case);
        Assert.Equal("Singular", result.Number);
        Assert.Equal("Feminine", result.Gender);
    }

    #endregion
}
