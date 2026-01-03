#!/usr/bin/env python3
"""
Script to update strong_def values in verses.json using definitions from strongs_cleaned.json.

This script:
1. Reads the strongs_cleaned.json file containing Strong's number definitions
2. Reads the verses.json file containing verse tokens with strongs references
3. Updates each token's strong_def field with the corresponding definition from strongs_cleaned.json
4. Saves the updated verses.json file
"""

import json
from pathlib import Path


def load_json_file(file_path: Path) -> dict:
    """Load and return JSON data from a file."""
    with open(file_path, 'r', encoding='utf-8') as f:
        return json.load(f)


def save_json_file(file_path: Path, data: dict) -> None:
    """Save JSON data to a file with proper formatting."""
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, indent=2)


def update_strong_definitions(verses: dict, strongs_definitions: dict) -> tuple[dict, int, int, set]:
    """
    Update strong_def fields in verses using definitions from strongs_definitions.
    
    Returns:
        tuple: (updated_verses, updated_count, not_found_count, missing_strongs_set)
    """
    updated_count = 0
    not_found_count = 0
    missing_strongs = set()
    
    for verse_ref, verse_data in verses.items():
        if 'tokens' not in verse_data:
            continue
            
        for token in verse_data['tokens']:
            strongs_num = token.get('strongs')
            if not strongs_num:
                continue
                
            if strongs_num in strongs_definitions:
                new_def = strongs_definitions[strongs_num]
                old_def = token.get('strong_def', '')
                if old_def != new_def:
                    token['strong_def'] = new_def
                    updated_count += 1
            else:
                not_found_count += 1
                missing_strongs.add(strongs_num)
    
    return verses, updated_count, not_found_count, missing_strongs


def main():
    """Main entry point for the script."""
    base_path = Path(__file__).parent
    
    strongs_file = base_path / 'strongs_cleaned.json'
    verses_file = base_path / 'verses.json'
    
    print(f"Loading Strong's definitions from {strongs_file}...")
    strongs_definitions = load_json_file(strongs_file)
    print(f"  Loaded {len(strongs_definitions)} Strong's definitions")
    
    print(f"\nLoading verses from {verses_file}...")
    verses = load_json_file(verses_file)
    print(f"  Loaded {len(verses)} verses")
    
    print("\nUpdating strong_def fields...")
    updated_verses, updated_count, not_found_count, missing_strongs = update_strong_definitions(
        verses, strongs_definitions
    )
    
    print(f"\nSaving updated verses to {verses_file}...")
    save_json_file(verses_file, updated_verses)
    
    print("\n" + "=" * 60)
    print("Summary:")
    print(f"  - Definitions updated: {updated_count}")
    print(f"  - Strong's numbers not found in strongs_cleaned.json: {not_found_count}")
    
    if missing_strongs:
        print(f"\n  Missing Strong's numbers ({len(missing_strongs)} unique):")
        for strongs_num in sorted(missing_strongs)[:20]:
            print(f"    - {strongs_num}")
        if len(missing_strongs) > 20:
            print(f"    ... and {len(missing_strongs) - 20} more")
    
    print("\nDone!")


if __name__ == '__main__':
    main()
