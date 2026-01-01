# LogosAPI Application Architecture & Purpose

**Date:** January 1, 2026  
**Purpose:** ChatGPT Custom GPT Backend API for Biblical Text Analysis

---

## Application Purpose

LogosAPI is a Web API designed to serve as the backend for a **Custom GPT in ChatGPT**. It provides biblical verse lookup with integrated Greek/Hebrew lexicon definitions, enabling AI-powered biblical text analysis.

---

## Core Workflow

### Request Flow
1. **Custom GPT receives user query** about Bible verses
2. **GPT sends array of verse references** to LogosAPI (e.g., `["Matt.1.1", "John.3.16"]`)
3. **API looks up each verse** in `ConcurrentDictionary<string, VerseData>`
4. **API extracts Strong's numbers** from verse tokens
5. **API looks up lexicon definitions** in `ConcurrentDictionary<string, string>`
6. **API returns enriched response** with verses + lexicon definitions
7. **GPT uses data** to provide theological insights to user

---

## Data Structures

### verses.json Structure

**Format:** JSON dictionary with verse references as keys

```json
{
  "Matt.1.1": {
    "tokens": [
      {
        "gloss": "book",
        "greek": "Βίβλος",
        "strongs": "G976",
        "rmac": "N-NSF",
        "rmac_desc": "Noun, Nominative, Singular, Feminine"
      },
      {
        "gloss": "origin",
        "greek": "γενέσεως",
        "strongs": "G1078",
        "rmac": "N-GSF",
        "rmac_desc": "Noun, Genitive, Singular, Feminine"
      }
    ]
  }
}
```

**Key Components:**
- **Verse Reference:** Format `{Book}.{Chapter}.{Verse}` (e.g., `Matt.1.1`)
- **Tokens Array:** Each word in the verse with linguistic data
- **gloss:** English translation of the word
- **greek:** Original Greek text (or Hebrew for OT)
- **strongs:** Strong's Concordance number (e.g., `G976` for Greek, `H1234` for Hebrew)
- **rmac:** Robinson's Morphological Analysis Code
- **rmac_desc:** Human-readable morphology description

### lexicon.json Structure

**Format:** JSON dictionary with Strong's numbers as keys

```json
{
  "G976": "Βίβλος [G:N-F]\nbook — βίβλος, -ου, ἡ\n[in LXX chiefly for סֵפֶר;]\na book, scroll...",
  "G1078": "γένεσις [G:N-F]\norigin — γένεσις, -εως, ἡ\n[in LXX for תּוֹלְדוֹת;]\ngeneration, birth, origin..."
}
```

**Key Components:**
- **Strong's Number:** Key (e.g., `G976`, `H1234`)
- **Definition:** Multi-line text with:
  - Original word in Greek/Hebrew
  - English gloss
  - Hebrew/Aramaic equivalents from LXX
  - Detailed definition
  - Biblical references where used

---

## In-Memory Architecture

### Why ConcurrentDictionary?

The application loads both JSON files into memory as `ConcurrentDictionary` objects for:
1. **Fast lookups** - O(1) access time
2. **Thread-safety** - Multiple concurrent requests from ChatGPT
3. **No I/O overhead** - Data loaded once at startup
4. **Memory efficiency** - Optimized for 1GB container

### Memory Footprint

| Data | Size | Est. Memory |
|------|------|-------------|
| verses.json | Variable | ~50-100MB |
| lexicon.json | Variable | ~20-50MB |
| **Total** | | **~100-150MB** |

This leaves ~800-850MB for request processing on a 1GB Koyeb instance.

---

## API Endpoints (To Be Implemented)

### 1. Verse Lookup Endpoint

**POST** `/api/verses/lookup`

**Request:**
```json
{
  "verseReferences": ["Matt.1.1", "John.3.16", "Rom.3.23"]
}
```

**Response:**
```json
{
  "verses": [
    {
      "reference": "Matt.1.1",
      "tokens": [ /* token array */ ],
      "lexiconEntries": {
        "G976": "Βίβλος [G:N-F] book — ...",
        "G1078": "γένεσις [G:N-F] origin — ..."
      }
    }
  ]
}
```

### 2. Lexicon Lookup Endpoint

**POST** `/api/lexicon/lookup`

**Request:**
```json
{
  "strongsNumbers": ["G976", "G1078", "H1234"]
}
```

**Response:**
```json
{
  "entries": {
    "G976": "Βίβλος [G:N-F] book — ...",
    "G1078": "γένεσις [G:N-F] origin — ..."
  }
}
```

### 3. Health Check

**GET** `/health`

Returns: `"Healthy"`

---

## Implementation Pattern

### Startup (Program.cs)

```csharp
// Singleton service for Bible data
builder.Services.AddSingleton<IBibleDataService, BibleDataService>();

// Service loads data on first access
public class BibleDataService : IBibleDataService
{
    private readonly ConcurrentDictionary<string, VerseData> _verses;
    private readonly ConcurrentDictionary<string, string> _lexicon;
    
    public BibleDataService(IWebHostEnvironment env)
    {
        // Load from /app/Data/verses.json
        _verses = LoadVerses();
        
        // Load from /app/Data/lexicon.json
        _lexicon = LoadLexicon();
    }
}
```

### Controller Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class VersesController : ControllerBase
{
    private readonly IBibleDataService _bibleData;
    
    [HttpPost("lookup")]
    public IActionResult LookupVerses([FromBody] VerseLookupRequest request)
    {
        var results = new List<VerseResponse>();
        
        foreach (var verseRef in request.VerseReferences)
        {
            // O(1) lookup in ConcurrentDictionary
            if (_bibleData.Verses.TryGetValue(verseRef, out var verse))
            {
                // Extract Strong's numbers from tokens
                var strongsNumbers = verse.Tokens
                    .Select(t => t.Strongs)
                    .Distinct()
                    .ToList();
                
                // O(1) lookup for each Strong's number
                var lexiconEntries = strongsNumbers
                    .Where(s => _bibleData.Lexicon.TryGetValue(s, out _))
                    .ToDictionary(
                        s => s,
                        s => _bibleData.Lexicon[s]
                    );
                
                results.Add(new VerseResponse
                {
                    Reference = verseRef,
                    Tokens = verse.Tokens,
                    LexiconEntries = lexiconEntries
                });
            }
        }
        
        return Ok(new { verses = results });
    }
}
```

---

## Model Classes

### VerseData
```csharp
public class VerseData
{
    public List<TokenData> Tokens { get; set; }
}

public class TokenData
{
    public string Gloss { get; set; }
    public string Greek { get; set; }      // or Hebrew
    public string Strongs { get; set; }
    public string Rmac { get; set; }
    public string RmacDesc { get; set; }
}
```

### Request/Response DTOs
```csharp
public class VerseLookupRequest
{
    public List<string> VerseReferences { get; set; }
}

public class VerseResponse
{
    public string Reference { get; set; }
    public List<TokenData> Tokens { get; set; }
    public Dictionary<string, string> LexiconEntries { get; set; }
}
```

---

## Performance Considerations

### Startup Time
- **First request:** ~1-3 seconds (loading JSON into memory)
- **Subsequent requests:** <50ms (in-memory lookups)

### Memory Management
- **GC Heap Limit:** 953MB (configured via `DOTNET_GCHeapHardLimit`)
- **Data loaded once** at application startup
- **No garbage** - ConcurrentDictionary is long-lived
- **Thread-safe** - No locking overhead

### Scalability
- **Concurrent requests:** Limited by 1GB RAM
- **Request throughput:** High (all data in memory)
- **Network latency:** Main bottleneck (ChatGPT → Koyeb)

---

## Custom GPT Integration

### GPT Configuration

**Instructions for Custom GPT:**
```
You are a biblical scholar assistant with access to the original Greek/Hebrew text.

When a user asks about Bible verses:
1. Identify verse references (e.g., Matthew 1:1, John 3:16)
2. Call the LogosAPI with the verse references
3. Use the token data to show original language words
4. Use the lexicon entries to explain word meanings
5. Provide theological insights based on the original text
```

**API Schema for GPT:**
```yaml
openapi: 3.0.0
info:
  title: LogosAPI
  version: 1.0.0
servers:
  - url: https://your-app.koyeb.app
paths:
  /api/verses/lookup:
    post:
      summary: Look up Bible verses with lexicon data
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                verseReferences:
                  type: array
                  items:
                    type: string
                  example: ["Matt.1.1", "John.3.16"]
```

---

## Deployment Notes

- **Data files must be included** in Docker image at `/app/Data/`
- **Files are read-only** after startup
- **No database required** - all data in memory
- **Memory limit critical** - ensure GC heap limit is set
- **Startup probe needed** - first request is slow while loading data

---

## Future Enhancements

1. **Caching layer** - Redis for frequently requested verses
2. **Batch lookup optimization** - Parallel dictionary lookups
3. **Compression** - Gzip JSON responses
4. **Analytics** - Track most-requested verses
5. **Search endpoint** - Find verses by keyword/Strong's number
6. **Hebrew/Aramaic support** - Currently Greek-focused

---

## Related Memories

- `project-overview.md` - Project structure
- `koyeb-deployment-guide.md` - Deployment instructions
- `koyeb-troubleshooting.md` - Common issues
