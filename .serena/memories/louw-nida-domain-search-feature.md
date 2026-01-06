# Louw-Nida Semantic Domain Search Feature

**Date Created:** January 6, 2026
**Status:** ✅ COMPLETED (January 6, 2026)

## Feature Overview

Create a new API endpoint that allows searching for words and verses by Louw-Nida semantic domain. This enables users to find thematically related Greek words across the New Testament.

## Background Discussion Context

### What is Louw-Nida?
The Louw-Nida Greek-English Lexicon organizes Greek words by semantic domain (concept) rather than alphabetically. Each word is assigned a domain number like "57.235" where:
- **57** = Major domain (e.g., "Possess, Transfer, Exchange")
- **235** = Subdomain entry

### Key Insight: Multiple L-N Numbers
Some words have multiple Louw-Nida numbers (e.g., `"louwNida": "57.235 57.236"`), meaning the word has multiple senses or uses that fall into different semantic categories.

### User Requirements from Discussion
1. **Words need verse context** - A word alone doesn't convey meaning; it needs the verse it appears in
2. **Domain-based search** - Find all words in a semantic domain (e.g., domain 57 = economic terms)
3. **Example Use Case**: For Philippians 2:6 "something to be grasped" (ἁρπαγμόν, G725), find the top 10 semantically related words

### Example Response (Contrived for Phil 2:6)
User asks about ἁρπαγμόν (harpagmon) in Phil 2:6 - Domain 57.235 (Steal, Rob)
Top 10 related words in domain 57 (Possess, Transfer, Exchange):
1. **κλέπτω** (kleptō) - "to steal" (57.232) - Matt 6:19 "where thieves break in and steal"
2. **ἁρπάζω** (harpazō) - "to seize, snatch" (57.235) - John 10:28 "no one will snatch them"
3. **λῃστής** (lēstēs) - "robber" (57.239) - Luke 10:30 "fell among robbers"
4. **κλέπτης** (kleptēs) - "thief" (57.233) - John 10:1 "the same is a thief"
5. ... etc.

## Data Sources

### Current verses.json Token Structure
```json
{
  "gloss": "in/on/among",
  "greek": "Ἐν",
  "lemma": "ἐν",
  "domain": "067002",         // 6-digit domain code
  "louwNida": "67.33",        // L-N section number
  "strongs": "G1722"
}
```

### Domain Label Mapping
File: `LogosAPI/Data/marble-domain-label-mapping.json`
Contains human-readable labels for domain codes.

## Proposed Solution Architecture

### 1. New Data Dictionary Structure
Build a dictionary indexed by major domain number:
```json
{
  "57": {  // Major Domain: Possess, Transfer, Exchange
    "domainLabel": "Possess, Transfer, Exchange",
    "entries": [
      {
        "subdomain": "235",
        "lemma": "ἁρπαγμός",
        "gloss": "something to be grasped",
        "strongs": "G725",
        "frequency": 1,
        "isHapax": true,
        "verses": ["Phil.2.6"]
      },
      {
        "subdomain": "232",
        "lemma": "κλέπτω",
        "gloss": "to steal",
        "strongs": "G2813",
        "frequency": 13,
        "isHapax": false,
        "verses": ["Matt.6.19", "Matt.6.20", "Matt.19.18", ...]
      }
    ]
  }
}
```

### 2. New Service: ISemanticDomainService
```csharp
public interface ISemanticDomainService
{
    // Get all words in a major domain
    DomainSearchResult GetWordsByDomain(int majorDomain);
    
    // Get related words for a specific L-N number
    RelatedWordsResult GetRelatedWords(string louwNidaNumber, int limit = 10);
    
    // Get words with sample verses
    DomainSearchResult GetWordsByDomainWithVerses(int majorDomain, int versesPerWord = 3);
}
```

### 3. New Controller Endpoint
```
GET /api/semantic-domain/{majorDomain}
GET /api/semantic-domain/{majorDomain}/related?louwNida=57.235&limit=10
```

### 4. Response Model
```csharp
public class DomainSearchResult
{
    public int MajorDomain { get; set; }
    public string DomainLabel { get; set; }
    public List<DomainEntry> Entries { get; set; }
}

public class DomainEntry
{
    public string Subdomain { get; set; }
    public string Lemma { get; set; }
    public string Gloss { get; set; }
    public string Strongs { get; set; }
    public int Frequency { get; set; }
    public bool IsHapax { get; set; }
    public List<string> SampleVerses { get; set; }  // First N verses
}
```

## Implementation Plan

### Phase 1: Research & Data Analysis
1. Analyze verses.json to understand L-N data coverage
2. Determine how many unique domains exist
3. Identify words with multiple L-N numbers
4. Research best practices for semantic domain APIs

### Phase 2: Data Processing
1. Create Python script to build domain dictionary from verses.json
2. Generate `semantic-domains.json` data file
3. Validate data structure and coverage

### Phase 3: API Implementation
1. Create model classes (DomainSearchResult, DomainEntry, etc.)
2. Create ISemanticDomainService interface
3. Implement SemanticDomainService
4. Create SemanticDomainController
5. Register services in Program.cs

### Phase 4: Testing & Documentation
1. Write unit tests for service
2. Write integration tests for controller
3. Update OpenAPI spec
4. Update GPT instructions

## Key Design Decisions

1. **Index by major domain** - Allows browsing entire semantic categories
2. **Include verse references** - Context is critical for meaning
3. **Limit sample verses** - Prevent response bloat (configurable limit)
4. **Support related words query** - Primary use case from discussion
5. **Separate endpoint from lexicon** - Different purpose, different structure

## Files to Create/Modify

### New Files
- `LogosAPI/Models/DomainSearchResult.cs`
- `LogosAPI/Models/DomainEntry.cs`
- `LogosAPI/Models/RelatedWordsResult.cs`
- `LogosAPI/Services/ISemanticDomainService.cs`
- `LogosAPI/Services/SemanticDomainService.cs`
- `LogosAPI/Controllers/SemanticDomainController.cs`
- `LogosAPI/Data/semantic-domains.json` (generated)
- `build-semantic-domains.py` (data generation script)
- `LogosAPI.Tests/SemanticDomainServiceTests.cs`

### Modified Files
- `LogosAPI/Program.cs` - Register new service
- `openapi-spec.json` - Add new endpoint
- `greek-exegetical-assistant-instructions.md` - Document new capability

## Implementation Summary

### Completed Tasks
1. ✅ Analyzed verses.json: 92.1% L-N coverage, 93 domains, 7005 L-N numbers
2. ✅ Generated semantic-domains.json: 4.3MB, 9069 word entries
3. ✅ Created models: DomainSearchResult, DomainEntry, RelatedWordsResult
4. ✅ Created ISemanticDomainService interface
5. ✅ Implemented SemanticDomainService (loads from embedded resource)
6. ✅ Created SemanticDomainController with 3 endpoints
7. ✅ Registered service in Program.cs as Singleton
8. ✅ Added 26 unit tests (all passing)
9. ✅ Updated OpenAPI spec (v1.8.0)
10. ✅ Updated GPT instructions with SEMANTIC DOMAIN SEARCH MODE

### API Endpoints
- `GET /api/semantic-domain/domains` - List all 93 domain numbers
- `GET /api/semantic-domain/{majorDomain}` - Get all words in a domain
- `GET /api/semantic-domain/related?louwNida=X` - Find related words

### Test Results
- 320 total tests pass (294 existing + 26 new)
- Build succeeds with 0 warnings, 0 errors

### Files Created
- LogosAPI/Models/DomainSearchResult.cs
- LogosAPI/Models/DomainEntry.cs
- LogosAPI/Models/RelatedWordsResult.cs
- LogosAPI/Services/ISemanticDomainService.cs
- LogosAPI/Services/SemanticDomainService.cs
- LogosAPI/Controllers/SemanticDomainController.cs
- LogosAPI/Data/semantic-domains.json
- LogosAPI.Tests/SemanticDomainServiceTests.cs