# Bible Verse Reference Normalization Research

**Date:** January 1, 2026  
**Status:** Research Complete - Custom Implementation Recommended

---

## Research Findings

### Available Libraries

1. **@gracious.tech/bible-references (JavaScript)**
   - Full-featured parser and normalizer
   - Uses USX book codes (lowercase)
   - Not available for C#/.NET

2. **No C#/.NET Libraries Found**
   - No established NuGet packages for verse reference parsing
   - Custom implementation required

---

## Canonical Format in LogosAPI

**Pattern:** `{Book}.{Chapter}.{Verse}`

**Examples:**
- `Matt.1.1`
- `John.3.16`
- `1Cor.13.4`
- `Rev.22.21`

### Book Codes in verses.json

New Testament books (27 total):
```
Matt, Mark, Luke, John, Acts, Rom, 1Cor, 2Cor, Gal, Eph, 
Phil, Col, 1Thess, 2Thess, 1Tim, 2Tim, Titus, Phlm, Heb, 
Jas, 1Pet, 2Pet, 1John, 2John, 3John, Jude, Rev
```

---

## Input Formats to Normalize

The normalizer must handle these common input variations:

### Book Name Variations
| Input | Normalized |
|-------|------------|
| Matthew | Matt |
| Mat | Matt |
| Mt | Matt |
| matthew | Matt |
| MATTHEW | Matt |
| 1 Corinthians | 1Cor |
| 1 Cor | 1Cor |
| I Corinthians | 1Cor |
| 1st Corinthians | 1Cor |
| First Corinthians | 1Cor |

### Delimiter Variations
| Input | Normalized |
|-------|------------|
| Matt 1:1 | Matt.1.1 |
| Matt 1.1 | Matt.1.1 |
| Matt.1.1 | Matt.1.1 |
| Matthew 1:1 | Matt.1.1 |
| Matt 1 1 | Matt.1.1 |
| Matt1:1 | Matt.1.1 |

### Edge Cases
| Input | Normalized |
|-------|------------|
| MATT.1.1 | Matt.1.1 |
| matt.1.1 | Matt.1.1 |
| Matt.01.01 | Matt.1.1 |
| Matt. 1. 1 | Matt.1.1 |

---

## Normalization Algorithm

### Step 1: Tokenize Input
```
"Matthew 1:1" → ["Matthew", "1", "1"]
"1 Cor 13:4" → ["1", "Cor", "13", "4"]
```

### Step 2: Extract Book Name
- Handle numbered books (1, 2, 3, I, II, III, First, Second, Third)
- Combine number prefix with book name
- Example: ["1", "Cor", "13", "4"] → book="1Cor", chapter="13", verse="4"

### Step 3: Normalize Book Name
- Convert to lowercase for lookup
- Match against book alias dictionary
- Return canonical form

### Step 4: Format Output
```
"{Book}.{Chapter}.{Verse}"
```

---

## Book Alias Dictionary

```csharp
private static readonly Dictionary<string, string> BookAliases = new()
{
    // Matthew
    { "matthew", "Matt" },
    { "matt", "Matt" },
    { "mat", "Matt" },
    { "mt", "Matt" },
    
    // Mark
    { "mark", "Mark" },
    { "mrk", "Mark" },
    { "mk", "Mark" },
    
    // Luke
    { "luke", "Luke" },
    { "luk", "Luke" },
    { "lk", "Luke" },
    
    // John
    { "john", "John" },
    { "jhn", "John" },
    { "jn", "John" },
    
    // Acts
    { "acts", "Acts" },
    { "act", "Acts" },
    
    // Romans
    { "romans", "Rom" },
    { "rom", "Rom" },
    { "rm", "Rom" },
    
    // 1 Corinthians
    { "1corinthians", "1Cor" },
    { "1cor", "1Cor" },
    { "1co", "1Cor" },
    { "icorinthians", "1Cor" },
    { "firstcorinthians", "1Cor" },
    
    // ... etc for all books
};
```

---

## Implementation Strategy

### Option 1: Regex-Based Parser (Recommended)

**Pros:**
- Single regex can handle multiple formats
- Fast execution
- No external dependencies

**Pattern:**
```regex
^(?:(\d|I{1,3}|First|Second|Third)\s*)?([A-Za-z]+)[\s\.]*(\d+)[\s:\.]+(\d+)$
```

### Option 2: State Machine Parser

**Pros:**
- More readable
- Easier to debug
- Better error messages

**Cons:**
- More code
- Potentially slower

---

## Recommended: Custom VerseReferenceNormalizer

### Interface
```csharp
public interface IVerseReferenceNormalizer
{
    string Normalize(string input);
    bool TryNormalize(string input, out string normalized);
    bool IsValid(string input);
}
```

### Implementation Structure
```
Services/
├── IVerseReferenceNormalizer.cs    # Interface
└── VerseReferenceNormalizer.cs     # Implementation
    ├── Normalize()                  # Main method
    ├── TryNormalize()              # Safe method
    ├── ParseReference()            # Extract parts
    ├── NormalizeBookName()         # Book alias lookup
    └── FormatReference()           # Build canonical form
```

---

## Unit Test Cases

### Valid Inputs
```csharp
[Theory]
[InlineData("Matt.1.1", "Matt.1.1")]
[InlineData("Matthew 1:1", "Matt.1.1")]
[InlineData("Matt 1:1", "Matt.1.1")]
[InlineData("matt.1.1", "Matt.1.1")]
[InlineData("MATT.1.1", "Matt.1.1")]
[InlineData("Mt 1:1", "Matt.1.1")]
[InlineData("John 3:16", "John.3.16")]
[InlineData("Jn 3:16", "John.3.16")]
[InlineData("1 Cor 13:4", "1Cor.13.4")]
[InlineData("1Cor 13:4", "1Cor.13.4")]
[InlineData("I Corinthians 13:4", "1Cor.13.4")]
[InlineData("First Corinthians 13:4", "1Cor.13.4")]
[InlineData("Rev 22:21", "Rev.22.21")]
[InlineData("Revelation 22:21", "Rev.22.21")]
public void Normalize_ValidInput_ReturnsCanonical(string input, string expected)
```

### Invalid Inputs
```csharp
[Theory]
[InlineData("")]
[InlineData("   ")]
[InlineData("Invalid")]
[InlineData("Matt")]
[InlineData("Matt 1")]
[InlineData("Unknown 1:1")]
public void Normalize_InvalidInput_ThrowsException(string input)
```

### Edge Cases
```csharp
[Theory]
[InlineData("Matt.01.01", "Matt.1.1")]      // Leading zeros
[InlineData("  Matt 1:1  ", "Matt.1.1")]    // Whitespace
[InlineData("Matt. 1. 1", "Matt.1.1")]      // Extra spaces
[InlineData("3 John 1:1", "3John.1.1")]     // Numbered book
[InlineData("III John 1:1", "3John.1.1")]   // Roman numeral
public void Normalize_EdgeCases_HandlesCorrectly(string input, string expected)
```

---

## Performance Considerations

1. **Book alias dictionary** - Use `StringComparer.OrdinalIgnoreCase` for fast lookup
2. **Regex compilation** - Compile regex once, reuse (singleton)
3. **No allocations** - Use `Span<char>` where possible
4. **Thread-safe** - Stateless normalizer is inherently thread-safe

---

## Related Memories

- `application-architecture.md` - System design
- `coding-standards.md` - Coding standards
- `gpt-actions-guide.md` - API integration
