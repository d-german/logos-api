# Design Patterns and Guidelines: Logos API

## SOLID Principles (Mandatory)

### Single Responsibility Principle (SRP)
- Each service has ONE clear purpose
- Example: `StrongsNumberNormalizer` only handles Strong's number normalization
- Example: `BibleDataService` only loads and provides data access
- Controllers only handle HTTP concerns, delegate to services

### Open/Closed Principle
- Extend behavior through interfaces and inheritance, not modification
- Use strategy pattern where multiple implementations exist
- Sealed classes prevent unwanted inheritance

### Liskov Substitution
- All interface implementations must be substitutable
- Mock implementations in tests follow same contracts

### Interface Segregation
- Keep interfaces small and focused (e.g., `IVerseReferenceNormalizer` has only 3 methods)
- Don't force clients to depend on methods they don't use

### Dependency Inversion
- All dependencies are injected via constructor
- Controllers depend on interfaces, not concrete implementations
- Services registered in DI container

## Design Patterns in Use

### Singleton Pattern
All services are registered as singletons for performance and memory efficiency:
- Data is loaded once at startup
- Thread-safe via ConcurrentDictionary
- Appropriate for stateless, read-only services

### Try-Parse Pattern
Services like normalizers use the Try-Parse pattern:
```csharp
public bool TryNormalize(string input, out string? normalized)
{
    // Returns true/false, outputs result via out parameter
}
```

### Service Layer Pattern
Controllers → Services → Data
- Controllers handle HTTP concerns
- Services contain business logic
- Models define data contracts

### Factory Pattern
- `BibleDataService` uses factory methods internally for JSON parsing
- Services create and return domain models

## Key Design Patterns
1. **Dependency Injection**: Constructor injection throughout
2. **Interface Segregation**: Focused, single-purpose interfaces
3. **Singleton Pattern**: For data-heavy services
4. **Factory Pattern**: JSON deserialization with custom options
5. **Try-Parse Pattern**: `TryNormalize()`, `TryParse()` for safe parsing
6. **Repository Pattern** (implicit): Services abstract data access

## Important Guidelines
- **Immutability**: Prefer immutable data structures (init-only properties)
- **Pure Functions**: Favor methods without side effects where possible
- **Interface Segregation**: Keep interfaces focused and minimal
- **Dependency Inversion**: Depend on abstractions (interfaces), not concrete types
- **Open/Closed**: Extend through composition, not modification
