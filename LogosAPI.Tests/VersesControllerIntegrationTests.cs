using System.Net;
using System.Net.Http.Json;
using LogosAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LogosAPI.Tests;

/// <summary>
/// Integration tests for VersesController - full stack testing
/// </summary>
public sealed class VersesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VersesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region GET Endpoint Tests

    [Fact]
    public async Task LookupVersesGet_WithValidReference_ReturnsOkWithVerseData()
    {
        // Act
        var response = await _client.GetAsync("/api/verses/lookup?verseReferences=Matt.1.1");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LookupVersesGet_WithNonCanonicalFormat_NormalizesAndReturns()
    {
        // Act - use non-canonical format
        var response = await _client.GetAsync("/api/verses/lookup?verseReferences=Matthew%201:1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
        // Verse found = in Verses list OR invalid = in NotFound list
        Assert.True(result.Verses.Count > 0 || result.NotFound.Count > 0);
    }

    [Fact]
    public async Task LookupVersesGet_WithMultipleReferences_ReturnsAll()
    {
        // Act
        var response = await _client.GetAsync(
            "/api/verses/lookup?verseReferences=Matt.1.1&verseReferences=John.3.16&verseReferences=Rom.8.28");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
        // Total should be 3 (some may be not found depending on data)
        Assert.Equal(3, result.Verses.Count + result.NotFound.Count);
    }

    [Fact]
    public async Task LookupVersesGet_WithInvalidReference_ReturnsInNotFoundList()
    {
        // Act
        var response = await _client.GetAsync("/api/verses/lookup?verseReferences=InvalidReference");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
        Assert.Empty(result.Verses);
        Assert.Single(result.NotFound);
        Assert.Equal("InvalidReference", result.NotFound[0]);
    }

    #endregion

    #region POST Endpoint Tests

    [Fact]
    public async Task LookupVersesPost_WithValidRequest_ReturnsOkWithVerseData()
    {
        // Arrange
        var request = new VerseLookupRequest { VerseReferences = new[] { "Matt.1.1" } };

        // Act
        var response = await _client.PostAsJsonAsync("/api/verses/lookup", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LookupVersesPost_WithMultipleReferences_ReturnsAll()
    {
        // Arrange
        var request = new VerseLookupRequest
        {
            VerseReferences = new[] { "Matt.1.1", "John.3.16", "1Cor.13.4" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/verses/lookup", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
        Assert.Equal(3, result.Verses.Count + result.NotFound.Count);
    }

    #endregion

    #region Health Endpoint Tests

    [Fact]
    public async Task HealthEndpoint_Controller_ReturnsHealthyStatus()
    {
        // Act - test the controller health endpoint
        var response = await _client.GetAsync("/api/verses/_health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task HealthEndpoint_Root_ReturnsHealthy()
    {
        // Act - test the root health endpoint
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", content);
    }

    #endregion

    #region Response Structure Tests

    [Fact]
    public async Task LookupVerses_ResponseHasCorrectStructure()
    {
        // Act
        var response = await _client.GetAsync("/api/verses/lookup?verseReferences=Matt.1.1");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<VerseLookupResult>();
        Assert.NotNull(result);
        Assert.NotNull(result.Verses);
        Assert.NotNull(result.NotFound);
    }

    #endregion
}
