# Fronting Enhancement Task - Context for Next Session

**Date Created:** January 5, 2026
**Status:** ✅ COMPLETED - January 5, 2026

## Problem Statement

The `fronting.json` data file contains word order analysis notes that currently show Greek text without English translations or Strong's numbers. This makes the notes difficult for non-Greek readers to understand.

**Current Format Example (Luke.14.28):**
```json
{
  "hasFronting": true,
  "clauses": [{
    "frontedElement": "πύργον οἰκοδομῆσαι",
    "frontedElementGloss": "tower to build",  // <-- Gloss exists in clause!
    "frontedRole": "object"
  }],
  "note": "**πύργον οἰκοδομῆσαι** (object) is fronted before the verb for emphasis."
}
```

**Desired Format:**
```json
{
  "note": "**πύργον οἰκοδομῆσαι** (tower to build, G4444+G3618) is fronted before the verb for emphasis."
}
```

## Task List (5 Tasks)

### Task 1: Create enrich_fronting_notes.py script
- Create Python script to enrich fronting.json notes
- Load fronting.json and verses.json
- Build Greek→(gloss, strongs) mapping from verse tokens
- Parse note field, find `**Greek**` patterns
- Replace with `**Greek** (English, Strongs)` format
- Use `frontedElementGloss` from clause as primary source
- Fall back to verses.json token lookup

### Task 2: Run enrichment script and validate output
- Backup original fronting.json
- Run enrich_fronting_notes.py
- Validate JSON structure
- Spot-check verses (Matt.1.19, Matt.1.20, Luke.14.28)

### Task 3: Update GPT instructions for enhanced format
- Update greek-exegetical-assistant-instructions.md
- Document new format in DATA RULES
- Update Word Order Notes example

### Task 4: Build and test API
- `dotnet build`
- `dotnet test` (all 282 tests should pass)
- Test endpoint with sample verse

### Task 5: Cleanup temporary files
- Delete script after use
- Update Serena memory

## Key Files

| File | Role |
|------|------|
| `LogosAPI/Data/fronting.json` | Target file (2,240 verse entries) |
| `LogosAPI/Data/verses.json` | Source of gloss and strongs data |
| `update_strongs_in_verses.py` | Reference pattern for Python script |
| `greek-exegetical-assistant-instructions.md` | GPT instructions to update |

## Data Structure Reference

### fronting.json clause object:
```json
{
  "pattern": "O-V",
  "frontedElement": "πύργον οἰκοδομῆσαι",  // Greek
  "frontedElementGloss": "tower to build",   // English (use this!)
  "frontedRole": "object",
  "note": "Object fronted before verb..."
}
```

### verses.json token object:
```json
{
  "gloss": "tower",
  "greek": "πύργον",
  "strongs": "G4444",
  "translit": "pyrgon"
}
```

## Implementation Notes

1. **Primary source for English**: Use `frontedElementGloss` from clause objects
2. **Fallback**: Look up individual Greek words in verses.json tokens
3. **Multi-word phrases**: Join strongs with "+" (e.g., G4444+G3618)
4. **Format**: `**Greek** (English, Strongs)` - keep Greek prominent
5. **Pattern**: Follow existing `update_strongs_in_verses.py` style

## Verification Checklist

- [x] Script runs without errors
- [x] All **Greek** patterns enriched (2022 notes, 3283 enrichments)
- [x] JSON remains valid (2240 entries)
- [x] No data loss
- [x] 282 tests pass
- [x] GPT instructions updated

## Completion Summary

**Task Completed:** January 5, 2026

### Results
- **Notes enriched:** 2,022 (of 2,240 total)
- **Total enrichments:** 3,283
- **Format:** `**Greek** (English gloss, G####)`
- **Example:** `**πύργον οἰκοδομῆσαι** (tower to build, G4444+G3618)`

### Files Modified
- `LogosAPI/Data/fronting.json` - Enriched with glosses and Strong's numbers
- `greek-exegetical-assistant-instructions.md` - Updated DATA RULES and example

### Backup Location
- `LogosAPI/Data/fronting.json.bak` - Pre-enrichment backup (kept for safety)

## Related Recent Work

Prior to this task, the following features were added to the API:
- Louw-Nida Domain Glosses (`domainGloss` field)
- Word Frequency Data (`frequency`, `frequencyRank`, `isHapax` fields)
- Fronting Detection (the feature now being enhanced)

## To Start the Task

```bash
# In new session, use task manager to get task list
mcp_mcp-shrimp-ta_list_tasks(status: "pending")

# Then execute first task
mcp_mcp-shrimp-ta_execute_task(taskId: "f0023a87-19cb-4c59-993e-778a1b66b4b2")
```
