using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Application.Contracts.Repositories;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.AI.GetAiExplanationStream;

public class GetAiExplanationStreamHandler : IStreamRequestHandler<GetAiExplanationStream, string>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IQuestionRepository _questionRepository;

    public GetAiExplanationStreamHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory,
        IQuestionRepository questionRepository)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _questionRepository = questionRepository;
    }

    public async IAsyncEnumerable<string> Handle(GetAiExplanationStream request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var apiKey = _configuration["AI:GeminiApiKey"];
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:streamGenerateContent?alt=sse&key={apiKey}";

        var data = await _questionRepository.GetRequiredAiData(request.QuestionId, request.Locale);

        if (data == null) yield break;
        var lang = request.Locale.ToUpper();

        var targetLanguage = lang switch
        {
            "PL" => "Polskim",
            "EN" => "Angielskim (English)",
            "DE" => "Niemieckim (Deutsch)",
            "UA" => "Ukraińskim (Ukrainian)",
            _ => "Polskim"
        };

        var prompt = $"""
                      Jesteś Ekspertem od prawa jazdy. Twoim zadaniem jest wyjaśnienie pytania na prawo jazdy w oparciu o dostarczone dane.

                      # DANE WEJŚCIOWE:
                      1. Pytanie{data.QuestionContent}
                      2. Statyczne wyjaścnienie{data.StaticResponse}
                      3. Opis co dzieje się na obrazie{data.AiContext}
                      # Zasady
                         Odpowiedz w 1-3 zdaniach.
                      # STYL:
                      - Nie używaj Markdown (pogrubień, list)
                      - Nie odpowiadaj na pytania użytkownika nie związane z prawem jazdy i egzaminami
                      - Nie odpowiadaj na pytania z bezsensownymi ciągami znaków                      

                      # ZAPYTANIE UŻYTKOWNIKA:
                         {request.UserQuery}

                      ### Odpowiadaj w języku: {targetLanguage}
                      """;

        var payload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { temperature = 0.3, maxOutputTokens = 150 }
        };

        using var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync(url, payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            yield return $"API error ({response.StatusCode}): {error}";
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (!line.StartsWith("data: ")) continue;
            var json = line.Substring(6).Trim();

            if (json == "{}" || string.IsNullOrEmpty(json)) continue;

            string? textPart = null;
            try
            {
                var chunk = JsonSerializer.Deserialize<GeminiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                textPart = chunk?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            }
            catch
            {
                continue;
            }

            if (!string.IsNullOrEmpty(textPart))
            {
                yield return textPart;
            }
        }
    }
}