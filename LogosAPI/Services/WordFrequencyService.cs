using System.Collections.Frozen;
using System.Text.Json;
using LogosAPI.Models;

namespace LogosAPI.Services;

/// <summary>
/// Service that calculates and provides word frequency data from verses.json
/// </summary>
public sealed class WordFrequencyService : IWordFrequencyService
{
    private readonly FrozenDictionary<string, WordFrequencyInfo> _frequencyData;

    public WordFrequencyService(IBibleDataService bibleDataService)
    {
        ArgumentNullException.ThrowIfNull(bibleDataService);
        _frequencyData = BuildFrequencyData(bibleDataService.Verses);
    }

    /// <inheritdoc />
    public WordFrequencyInfo? GetFrequency(string? lemma)
    {
        if (string.IsNullOrWhiteSpace(lemma))
            return null;

        return _frequencyData.GetValueOrDefault(lemma);
    }

    private static FrozenDictionary<string, WordFrequencyInfo> BuildFrequencyData(
        IReadOnlyDictionary<string, VerseData> verses)
    {
        var lemmaCounts = CountLemmas(verses);
        var rankedFrequencies = AssignRanks(lemmaCounts);
        
        return rankedFrequencies.ToFrozenDictionary();
    }

    private static Dictionary<string, int> CountLemmas(IReadOnlyDictionary<string, VerseData> verses)
    {
        var counts = new Dictionary<string, int>();
        
        foreach (var verse in verses.Values)
        {
            foreach (var token in verse.Tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Lemma))
                    continue;
                
                counts.TryGetValue(token.Lemma, out var currentCount);
                counts[token.Lemma] = currentCount + 1;
            }
        }
        
        return counts;
    }

    private static Dictionary<string, WordFrequencyInfo> AssignRanks(Dictionary<string, int> lemmaCounts)
    {
        var sortedByCount = lemmaCounts
            .OrderByDescending(kvp => kvp.Value)
            .ToList();

        var result = new Dictionary<string, WordFrequencyInfo>(sortedByCount.Count);
        var rank = 1;
        var previousCount = -1;
        var sameRankCount = 0;

        foreach (var (lemma, count) in sortedByCount)
        {
            // Handle ties - words with same count get same rank
            if (count != previousCount)
            {
                rank += sameRankCount;
                sameRankCount = 1;
            }
            else
            {
                sameRankCount++;
            }

            var isHapax = count == 1;
            result[lemma] = new WordFrequencyInfo(count, rank, isHapax);
            previousCount = count;
        }

        return result;
    }
}
