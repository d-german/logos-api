# Greek Exegetical Assistant Project

## Project Overview
Building a Greek Bible exegetical assistant GPT that provides scholarly analysis of Greek New Testament passages. The system uses a custom API (LogosAPI) hosted at `https://wispy-koi-d-german-8684f0e9.koyeb.app`.

## Current State (January 4, 2026)

### API Structure
- **Base URL:** `https://wispy-koi-d-german-8684f0e9.koyeb.app`
- **Verse Lookup:** `/api/verses/lookup?verseReferences=John.3.16`
- **Reference Format:** `Book.Chapter.Verse` (e.g., `Matt.1.1`, `John.3.16`, `Phil.2.6`)

### Token Response Structure
```json
{
  "gloss": "to weep",
  "greek": "Ἐδάκρυσεν",
  "translit": "edakrysen",
  "strongs": {
    "number": "G1145",
    "definition": "from 1144; to shed tears"
  },
  "rmac": "V-AAI-3S",
  "rmacDesc": "Verb, Aorist, Active, Indicative, third, Singular",
  "morph": { ... }
}
```

### Key Files
- **Instructions:** `greek-exegetical-assistant-instructions.md` (7392 chars, under 8000 limit)
- **API Project:** `C:\projects\github\logos-api\`
- **Models:** `TokenResponse.cs`, `StrongsInfo.cs`, `TokenData.cs`
- **Services:** `VerseLookupService.cs`

### Recent Changes Made
1. **StrongsInfo model created** - Nested structure with `number` and `definition` properties
2. **TokenResponse updated** - Changed `Strongs` from string to `StrongsInfo` object
3. **Gloss selection rule** - LLM picks best option from multi-choice glosses (e.g., "the/this/who" → "the")
4. **Strong's definition strict rule** - Must output COMPLETE definition, no truncation/ellipsis
5. **Token card format** - Mobile-first layout with H3 gloss, bullet points for Greek/Strong's/Morphology
6. **Quick Gloss section** - Single line showing all glosses in Greek word order with Strong's numbers
7. **Word Order Notes section** - Optional section for fronting/emphasis (only when present)

### Fronting Detection - NEXT MAJOR FEATURE

#### Problem
The LLM cannot reliably detect Greek fronting/marked word order. It missed Colossians 1:17 fronting.

#### Solution Found
**Clear-Bible/macula-greek** repository has syntax trees with fronting information:
- URL: `https://github.com/Clear-Bible/macula-greek`
- Data format: XML files in `Nestle1904/lowfat/` directory
- Each book has its own file (e.g., `04-john.xml`, `12-colossians.xml`)

#### How Fronting is Encoded
The `rule` attribute on `<wg class="cl">` elements indicates constituent order:
- `S-VC-P` = Subject-Verb-Predicate (NORMAL)
- `P-VC-S` = Predicate-Verb-Subject (FRONTED predicate - emphasis)
- `S-V-O` = Subject-Verb-Object (NORMAL transitive)
- `O-V-S` = Object-Verb-Subject (FRONTED object - emphasis)

#### Example from John 1:1
```xml
<!-- Clause 2: Normal order -->
<wg class="cl" rule="S-VC-P">...</wg>

<!-- Clause 3: FRONTED - θεὸς before ἦν -->
<wg class="cl" rule="P-VC-S">
   <w role="p">Θεὸς</w>      <!-- Predicate FIRST = fronted -->
   <w role="vc">ἦν</w>
   <wg role="s">ὁ Λόγος</wg>
</wg>
```

#### Verse Reference Format in XML
- `<milestone unit="verse" id="JHN 1:1">JHN 1:1</milestone>`
- Each word has `ref="JHN 1:1!1"` (book chapter:verse!word_number)

### Integration Plan (To Be Implemented)
1. Download/parse Macula-Greek XML files
2. Extract fronting information for each verse
3. Create lookup table or add `fronting` field to verse data
4. Update API to include fronting info in response
5. Update GPT instructions to use the fronting data

### Test Verses for Fronting
| Verse | Expected | Fronting Type |
|-------|----------|---------------|
| John 1:1 | YES | θεὸς fronted (P-VC-S) |
| John 1:14 | YES | λόγος/σάρξ fronted |
| Colossians 1:17 | YES | αὐτός emphatic, ἐν αὐτῷ fronted |
| Romans 8:28 | YES | πάντα fronted |
| Philippians 2:6 | YES | Prepositional phrase fronted |
| Galatians 2:20 | YES | Emphatic pronouns |
| John 6:35 | YES | Emphatic ἐγώ |
| John 3:16 | NO | Normal VSO order |
| John 11:35 | NO | Normal SV order |

### GPT Instructions Character Count
Current: **7392 characters** (limit: 8000)
Room remaining: 608 characters

### Key Formatting Rules in Instructions
1. **Quick Gloss:** Bold glosses with Strong's numbers in parentheses
2. **Token Cards:** H3 + bold-italic gloss, bullet points for data
3. **Strong's Definition:** NEVER truncate, output character-for-character
4. **Gloss Selection:** Pick single best option from multi-choice
5. **Word Order Notes:** Only include if fronting detected

## Commands for Testing
```powershell
# Test API endpoint
Invoke-RestMethod -Uri "https://wispy-koi-d-german-8684f0e9.koyeb.app/api/verses/lookup?verseReferences=John.1.1"

# Build project
cd C:\projects\github\logos-api\LogosAPI && dotnet build

# Run tests
cd C:\projects\github\logos-api\LogosAPI.Tests && dotnet test

# Check character count
(Get-Content "C:\projects\github\logos-api\greek-exegetical-assistant-instructions.md" -Raw).Length
```
