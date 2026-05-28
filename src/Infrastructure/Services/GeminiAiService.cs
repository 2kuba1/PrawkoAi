using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Application.Contracts.Services;
using Application.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public sealed class GeminiAiService : IAiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GeminiAiService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string?> GenerateContentAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        var apiKey = _configuration["AI:GeminiApiKey"];
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:generateContent?key={apiKey}";

        var payload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new
            {
                temperature = 0.5,
                maxOutputTokens = 400,
                topP = 0.8
            }
        };

        using var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync(url, payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(
            cancellationToken: cancellationToken);

        return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
    }

    public async IAsyncEnumerable<string> StreamContentAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var apiKey = _configuration["AI:GeminiApiKey"];
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:streamGenerateContent?alt=sse&key={apiKey}";

        var payload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new
            {
                temperature = 0.3,
                maxOutputTokens = 150
            }
        };

        using var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync(url, payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
            yield break;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data: "))
                continue;

            var json = line[6..].Trim();

            if (json == "{}" || string.IsNullOrEmpty(json))
                continue;

            GeminiResponse? chunk;

            try
            {
                chunk = JsonSerializer.Deserialize<GeminiResponse>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                continue;
            }

            var text = chunk?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (!string.IsNullOrWhiteSpace(text))
                yield return text;
        }
    }
}