# Code Style and Conventions: Logos API

## General Principles
- **SOLID Design**: All code strictly follows SOLID principles
- **Functional Programming**: Favor functional paradigms, immutability, pure functions, and higher-order functions
- **Static Methods**: Methods that don't access instance state MUST be declared static
- **Sealed Classes**: Use `sealed` for classes that shouldn't be inherited (e.g., StrongsNumberNormalizer, VerseData)
- **Single Responsibility**: Each class/method has one clear purpose

## C# Language Features
- **Nullable Reference Types**: Enabled project-wide (`<Nullable>enable</Nullable>`)
- **Required Properties**: Use `required` keyword for essential properties (e.g., `required List<TokenData> Tokens`)
- **Init-only Properties**: Prefer `init` over `set` for immutability
- **Partial Methods**: Use for generated regex patterns (`[GeneratedRegex]`)
- **File-scoped Namespaces**: Use file-scoped namespace declarations (not shown in samples but preferred)

## Naming Conventions
- **Interfaces**: Prefix with `I` (e.g., `IBibleDataService`, `IVerseReferenceNormalizer`)
- **Private Fields**: Prefix with `_` (e.g., `_verses`, `_logger`, `_bibleDataService`)
- **Method Names**: PascalCase, descriptive verbs (e.g., `LoadVerses`, `TryNormalize`, `IsValid`)
- **Properties**: PascalCase, nouns (e.g., `VersesCount`, `IsInitialized`)
- **Parameters**: camelCase (e.g., `input`, `normalized`)

## Documentation
- **XML Comments**: Use for all public interfaces and methods
- **Summary Tags**: Document purpose and responsibility
- **Cyclomatic Complexity**: Document in comments when relevant for code quality tracking
- **Inherit Doc**: Use `/// <inheritdoc />` when implementing interface methods

## Service Layer Patterns
- **Singleton Lifetime**: All services registered as singletons for performance
- **Thread Safety**: Use ConcurrentDictionary for shared state
- **Stateless Services**: Prefer stateless services (parsers, normalizers)
- **Interface Segregation**: Keep interfaces focused and minimal
- **Constructor Injection**: Use DI for all dependencies

## JSON Handling
- **Null Value Handling**: `JsonIgnoreCondition.WhenWritingNull` configured globally
- **Property Names**: camelCase in JSON (ASP.NET Core default)

## Error Handling
- **Try Pattern**: Use `TryXxx(out result)` pattern for operations that may fail (e.g., `TryNormalize`)
- **Validation**: Throw `ArgumentException` for invalid inputs with clear messages
- **Logging**: Use structured logging with ILogger

## Testing Standards
- **xUnit**: Primary testing framework
- **Integration Tests**: Use `Microsoft.AspNetCore.Mvc.Testing` for API testing
- **Unit Tests**: Test individual services and normalizers
- **Naming**: Test classes end with `Tests` (e.g., `VerseLookupServiceTests`)
- **Data Integrity**: Include data integrity tests for embedded resources

## Performance Considerations
- **Regex Compilation**: Use `[GeneratedRegex]` for compiled regex patterns
- **Lazy Initialization**: Consider lazy loading for expensive resources
- **Memory Efficiency**: ConcurrentDictionary for fast lookups without locks
- **GC Optimization**: Memory-conscious patterns for containerized deployment
