using LogosAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LogosAPI.Tests;

/// <summary>
/// Unit tests for FrontingDataService
/// </summary>
public sealed class FrontingDataServiceTests
{
    private readonly FrontingDataService _service;

    public FrontingDataServiceTests()
    {
        var mockLogger = new Mock<ILogger<FrontingDataService>>();
        _service = new FrontingDataService(mockLogger.Object);
    }

    #region Initialization Tests

    [Fact]
    public void Constructor_LoadsFrontingData_IsInitializedTrue()
    {
        // Assert
        Assert.True(_service.IsInitialized);
    }

    [Fact]
    public void Constructor_LoadsFrontingData_FrontingCountGreaterThanZero()
    {
        // Assert
        Assert.True(_service.FrontingCount > 0);
    }

    #endregion

    #region GetFronting Tests

    [Fact]
    public void GetFronting_KnownFrontingVerse_ReturnsFrontingInfo()
    {
        // Act - John 1:1 is known to have fronting (θεὸς fronted)
        var result = _service.GetFronting("John.1.1");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasFronting);
        Assert.NotNull(result.Note);
        Assert.NotEmpty(result.Note);
    }

    [Fact]
    public void GetFronting_KnownFrontingVerse_HasClauses()
    {
        // Act
        var result = _service.GetFronting("John.1.1");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Clauses);
        Assert.NotEmpty(result.Clauses);
    }

    [Fact]
    public void GetFronting_NonFrontingVerse_ReturnsNull()
    {
        // Act - John 3:16 has normal word order (no fronting)
        var result = _service.GetFronting("John.3.16");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFronting_NonExistentVerse_ReturnsNull()
    {
        // Act
        var result = _service.GetFronting("NotABook.99.99");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFronting_NullReference_ReturnsNull()
    {
        // Act
        var result = _service.GetFronting(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFronting_EmptyReference_ReturnsNull()
    {
        // Act
        var result = _service.GetFronting("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFronting_WhitespaceReference_ReturnsNull()
    {
        // Act
        var result = _service.GetFronting("   ");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region HasFronting Tests

    [Fact]
    public void HasFronting_KnownFrontingVerse_ReturnsTrue()
    {
        // Act
        var result = _service.HasFronting("John.1.1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFronting_NonFrontingVerse_ReturnsFalse()
    {
        // Act
        var result = _service.HasFronting("John.3.16");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasFronting_NonExistentVerse_ReturnsFalse()
    {
        // Act
        var result = _service.HasFronting("NotABook.99.99");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasFronting_NullReference_ReturnsFalse()
    {
        // Act
        var result = _service.HasFronting(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Clause Data Tests

    [Fact]
    public void GetFronting_John11_HasPVCSPattern()
    {
        // Act
        var result = _service.GetFronting("John.1.1");

        // Assert
        Assert.NotNull(result?.Clauses);
        Assert.Contains(result.Clauses, c => c.Pattern == "P-VC-S");
    }

    [Fact]
    public void GetFronting_John11_HasFrontedElement()
    {
        // Act
        var result = _service.GetFronting("John.1.1");

        // Assert
        Assert.NotNull(result?.Clauses);
        Assert.Contains(result.Clauses, c => !string.IsNullOrEmpty(c.FrontedElement));
    }

    #endregion
}
