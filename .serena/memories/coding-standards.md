# LogosAPI Coding Standards

**Date:** January 1, 2026  
**Applies To:** All C# code in LogosAPI project

---

## Core Principles

### 1. SOLID Principles (Mandatory)

All code must strictly follow SOLID principles:

#### Single Responsibility Principle (SRP)
- Each class has ONE reason to change
- Controllers handle HTTP requests only
- Services handle business logic
- Repositories handle data access
- Models represent data structures only

**Example:**
```csharp
// ✅ GOOD - Single responsibility
public sealed class VersesController : ControllerBase
{
    // Only handles HTTP request/response
}

// ❌ BAD - Multiple responsibilities
public class VersesController : ControllerBase
{
    // Handles HTTP, database access, and business logic
}
```

#### Open/Closed Principle (OCP)
- Open for extension, closed for modification
- Use interfaces and abstract classes
- Extend behavior without changing existing code

#### Liskov Substitution Principle (LSP)
- Derived classes must be substitutable for base classes
- Interfaces must be implemented fully

#### Interface Segregation Principle (ISP)
- Many specific interfaces better than one general interface
- Clients shouldn't depend on interfaces they don't use

#### Dependency Inversion Principle (DIP)
- Depend on abstractions, not concretions
- Use dependency injection
- Constructor injection preferred

**Example:**
```csharp
// ✅ GOOD - Depends on abstraction
public VersesController(ILogger<VersesController> logger)
{
    _logger = logger;
}
```

---

### 2. Functional Programming (Preferred)

Favor functional programming paradigms:

#### Immutability
- Use `required init` properties
- Use `record` types for DTOs
- Avoid mutable state

**Example:**
```csharp
// ✅ GOOD - Immutable
public sealed class VerseLookupRequest
{
    public required string[] VerseReferences { get; init; }
}

// ❌ BAD - Mutable
public class VerseLookupRequest
{
    public string[] VerseReferences { get; set; }
}
```

#### Pure Functions
- No side effects
- Same input always produces same output
- Deterministic behavior

**Example:**
```csharp
// ✅ GOOD - Pure function
private static VerseLookupResponse CreateResponse(string[] refs)
{
    return new VerseLookupResponse
    {
        Message = "Hello World",
        VerseReferences = refs
    };
}
```

#### Higher-Order Functions
- Functions as first-class citizens
- Use LINQ for data transformations
- Avoid loops where LINQ is clearer

---

### 3. Static Methods (Required)

**Rule:** Any method that does NOT access instance state MUST be static.

**Benefits:**
- Performance improvement
- Clarifies intent (no hidden dependencies)
- Better testability
- Enables reusability

**Example:**
```csharp
// ✅ GOOD - Static method (no instance state)
private static VerseLookupResponse CreateResponse(string[] refs)
{
    return new VerseLookupResponse { /* ... */ };
}

// ❌ BAD - Instance method when static would work
private VerseLookupResponse CreateResponse(string[] refs)
{
    return new VerseLookupResponse { /* ... */ };
}
```

**Exception:** Methods that use `_logger`, `_repository`, or other injected dependencies should remain instance methods.

---

### 4. Cyclomatic Complexity Limit

**Maximum Cyclomatic Complexity: 5**

Cyclomatic complexity measures the number of independent paths through code.

**Calculation:**
- Start at 1
- +1 for each `if`, `else if`, `while`, `for`, `foreach`
- +1 for each `case` in `switch`
- +1 for each `&&`, `||` in conditions
- +1 for each `catch` block

**Examples:**

```csharp
// ✅ GOOD - Complexity: 1
private static string FormatMessage(string text)
{
    return $"Result: {text}";
}

// ✅ GOOD - Complexity: 2
private static bool IsValidReference(string reference)
{
    if (string.IsNullOrEmpty(reference))
        return false;
    
    return reference.Contains(".");
}

// ✅ GOOD - Complexity: 3
private static string GetBookName(string reference)
{
    if (reference.StartsWith("Matt"))
        return "Matthew";
    if (reference.StartsWith("John"))
        return "John";
    return "Unknown";
}

// ❌ BAD - Complexity: 6 (too high!)
private static bool ValidateAndProcess(string ref)
{
    if (string.IsNullOrEmpty(ref)) // +1
        return false;
    
    if (ref.Contains(".") && ref.Length > 5) // +2
    {
        if (ref.StartsWith("Matt") || ref.StartsWith("John")) // +2
            return true;
    }
    
    return false;
}
```

**How to Fix High Complexity:**
1. **Extract methods** - Break into smaller functions
2. **Use strategy pattern** - Replace conditionals with polymorphism
3. **Use lookup tables** - Replace `if/else` chains with dictionaries
4. **Early returns** - Reduce nesting

**Refactored Example:**
```csharp
// ✅ GOOD - Split into multiple methods
private static bool ValidateReference(string reference)
{
    if (IsNullOrEmpty(reference))
        return false;
    
    if (!HasValidFormat(reference))
        return false;
    
    return IsSupportedBook(reference);
}

private static bool IsNullOrEmpty(string value) => 
    string.IsNullOrEmpty(value);

private static bool HasValidFormat(string reference) => 
    reference.Contains(".") && reference.Length > 5;

private static bool IsSupportedBook(string reference) =>
    reference.StartsWith("Matt") || reference.StartsWith("John");
```

---

## Code Style Guidelines

### Naming Conventions

- **Classes**: PascalCase, descriptive nouns
  - `VersesController`, `VerseLookupRequest`
- **Interfaces**: PascalCase, starts with `I`
  - `IVerseRepository`, `ILexiconService`
- **Methods**: PascalCase, descriptive verbs
  - `LookupVerses()`, `CreateResponse()`
- **Private fields**: camelCase with underscore
  - `_logger`, `_repository`
- **Parameters**: camelCase
  - `verseReferences`, `request`
- **Local variables**: camelCase
  - `response`, `result`

### Access Modifiers

- Be explicit: Always specify `public`, `private`, `protected`, etc.
- Use `sealed` on classes not designed for inheritance
- Use `readonly` on fields set in constructor

### Null Safety

- Use nullable reference types (`#nullable enable`)
- Use `required` for mandatory properties
- Throw `ArgumentNullException` for null parameters in constructors

**Example:**
```csharp
public VersesController(ILogger<VersesController> logger)
{
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
}
```

### Logging

- Use structured logging with named parameters
- Log at appropriate levels:
  - `LogInformation` - Normal operations
  - `LogWarning` - Unexpected but recoverable
  - `LogError` - Errors requiring attention
  - `LogDebug` - Detailed diagnostic info

**Example:**
```csharp
_logger.LogInformation(
    "Received {Method} request for verse lookup. References: {References}",
    method,
    string.Join(", ", verseReferences));
```

### XML Documentation

- Add XML comments to all public APIs
- Document parameters, returns, and exceptions
- Include examples where helpful

**Example:**
```csharp
/// <summary>
/// Look up Bible verses by reference
/// </summary>
/// <param name="request">Request containing verse references</param>
/// <returns>Response with verse data</returns>
[HttpPost("lookup")]
public IActionResult LookupVersesPost([FromBody] VerseLookupRequest request)
```

---

## File Organization

```
LogosAPI/
├── Controllers/        # HTTP request handlers (thin)
├── Models/            # DTOs and request/response models
├── Services/          # Business logic (interfaces + implementations)
├── Repositories/      # Data access (interfaces + implementations)
├── Data/              # Static data files (JSON)
└── Program.cs         # Application startup
```

---

## Dependency Injection

Register services in `Program.cs`:

```csharp
// Singleton - One instance for app lifetime
builder.Services.AddSingleton<IBibleDataService, BibleDataService>();

// Scoped - One instance per request
builder.Services.AddScoped<IVerseService, VerseService>();

// Transient - New instance each time
builder.Services.AddTransient<IEmailService, EmailService>();
```

**Prefer Singleton** for:
- Stateless services
- Thread-safe services
- Services with expensive initialization

---

## Testing Considerations

- Keep methods small (easier to test)
- Pure functions are easiest to test
- Use interfaces for mocking
- Test one thing per test method

---

## Performance Guidelines

- Use `ConcurrentDictionary` for thread-safe collections
- Prefer `struct` for small, immutable types
- Use `ValueTask` for async methods that often complete synchronously
- Avoid allocations in hot paths

---

## Anti-Patterns to Avoid

❌ **God Classes** - Classes that do too much
❌ **Anemic Models** - Models with no behavior (use records instead)
❌ **Magic Numbers** - Use named constants
❌ **Deep Nesting** - Keep cyclomatic complexity low
❌ **Mutable Static State** - Thread-safety issues
❌ **Exception Swallowing** - Always log exceptions

---

## Code Review Checklist

Before committing code, verify:

- [ ] All methods have cyclomatic complexity ≤ 5
- [ ] SOLID principles followed
- [ ] Static methods used where applicable
- [ ] Immutable data structures (init, record)
- [ ] No mutable shared state
- [ ] Proper logging with structured parameters
- [ ] XML documentation on public APIs
- [ ] No compiler warnings
- [ ] Meaningful variable names
- [ ] Early returns to reduce nesting
- [ ] Dependency injection used correctly
- [ ] Null safety considered
- [ ] Thread safety verified (for shared state)

---

## Example: Well-Structured Controller

```csharp
/// <summary>
/// Controller for Bible verse operations
/// Single Responsibility: Handle HTTP requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class VersesController : ControllerBase
{
    private readonly ILogger<VersesController> _logger;
    private readonly IVerseService _verseService;

    public VersesController(
        ILogger<VersesController> logger,
        IVerseService verseService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _verseService = verseService ?? throw new ArgumentNullException(nameof(verseService));
    }

    /// <summary>
    /// Look up verses (POST)
    /// Cyclomatic Complexity: 2
    /// </summary>
    [HttpPost("lookup")]
    public IActionResult LookupPost([FromBody] VerseLookupRequest request)
    {
        LogRequest("POST", request.VerseReferences);
        
        var result = _verseService.LookupVerses(request.VerseReferences);
        
        return Ok(result);
    }

    /// <summary>
    /// Log incoming request
    /// Cyclomatic Complexity: 1
    /// </summary>
    private void LogRequest(string method, string[] references)
    {
        _logger.LogInformation(
            "Received {Method} request. References: {Count}",
            method,
            references.Length);
    }
}
```

---

## Related Memories

- `application-architecture.md` - System design
- `gpt-actions-guide.md` - API integration
- `project-overview.md` - Project structure
