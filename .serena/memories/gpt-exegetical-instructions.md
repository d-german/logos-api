# GPT Exegetical Assistant Instructions

## Overview

The `greek-exegetical-assistant-instructions.md` file contains comprehensive instructions for a GPT-based Greek Bible exegetical assistant. This document defines the behavior, output format, and data handling rules for analyzing Greek New Testament passages.

## File Location

`greek-exegetical-assistant-instructions.md` (project root)

## Purpose

This instruction set is designed to be used as GPT instructions (system prompt) for a custom GPT that provides scholarly exegetical analysis of Greek Bible passages using data from the LogosAPI.

## Key Features

### 1. Seven-Section Analysis Structure

For new passage analysis, the GPT outputs exactly 7 numbered sections:

1. **English Translation** - Smooth, accurate translation based on NA28/UBS5
2. **Cross References** - Top 5 relevant parallel passages with brief quotes
3. **Interpretation** - Consensus view plus significant alternative readings
4. **Historical-Cultural Context** - 2-4 sentences of relevant background
5. **Life Application** - 2-4 sentences of practical, text-driven application
6. **Textual Criticism** - Major variants affecting meaning (when available)
7. **Interlinear Morphology + Grammar Legend** - Detailed word-by-word analysis

### 2. Data Integrity Rules (Critical)

- Verse token data (greek, gloss, translit, strongs, rmac, rmacDesc, morph) must be reproduced **verbatim**
- No spelling fixes, casing changes, or diacritic edits allowed
- Strong's definitions must be reproduced exactly
- Commentary text must be presented verbatim

### 3. Operating Modes

#### Passage Analysis Mode (Default)
- Triggered when user provides a Bible reference
- Outputs the full 7-section structure

#### Strong's Lookup Mode
- Triggered by queries like "G3056", "Strong's G3056", "definition of G3056"
- Output format: Strong's number on first line, then definition verbatim

#### Commentary Mode
- Triggered by requests like "Adam Clarke on Phil 2:6"
- Output format: Title line with commentary name and reference, then content verbatim

#### Follow-up Mode
- For clarifying questions about a previously analyzed passage
- Answers directly without repeating the 7-section structure

### 4. Token Card Format (Interlinear Section)

Each word token must follow this exact structure:

```
* {token.gloss}
  * {token.greek} ({token.translit})
  * {token.strongs}
    * {token.strongDef}
  * {token.rmac}
    * Implication: {1-2 sentences of grammatical/theological significance}
```

**Note:** If `strongDef` is null or missing, the Strong's definition line should be omitted entirely.

The "Implication" section is model-generated based on morphological data and provides insights about:
- Verbal aspects (present/aorist/perfect)
- Case usage (genitive/dative/accusative)
- Discourse function (particles, conjunctions, prepositions)

### 5. Grammar Legend

Immediately after the token cards, list each unique RMAC code in order of first appearance:
```
Grammar Legend
RMAC_CODE — {token.rmacDesc verbatim}
```

## API Integration

The GPT integrates with LogosAPI endpoints:
- `/api/verses` - Retrieve verse data with morphology
- `/api/lexicon` - Strong's dictionary lookups
- `/api/commentary` - Traditional biblical commentaries

## Design Principles

1. **Scholarly Precision** - Data integrity is paramount
2. **User-Friendly Presentation** - Clean formatting, no technical jargon about implementation
3. **Respectful of Traditions** - Avoids dogmatic positions, presents multiple views
4. **Practical Relevance** - Balances academic rigor with application

## Usage Context

This instruction set is designed for:
- Bible study apps and websites
- Seminary students and pastors
- Greek language learners
- Exegetical research

## Maintenance Notes

- Update this file if API schema changes (e.g., new token properties)
- Keep instructions concise and actionable
- Test with representative passages to ensure output quality
- Maintain strict data integrity rules to preserve scholarly value

## Related Files

- `LogosAPI/Models/TokenResponse.cs` - Defines the token data structure
- `LogosAPI/Controllers/VersesController.cs` - Verse data endpoint
- `LogosAPI/Controllers/LexiconController.cs` - Strong's lookup endpoint
- `LogosAPI/Controllers/CommentaryController.cs` - Commentary endpoint
