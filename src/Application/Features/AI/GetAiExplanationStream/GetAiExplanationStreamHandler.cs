using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.AI.GetAiExplanationStream;

public class GetAiExplanationStreamHandler : IStreamRequestHandler<GetAiExplanationStream, string>
{
    private readonly IConfiguration _configuration;
    private readonly IAiService _aiService;
    private readonly IQuestionRepository _questionRepository;

    public GetAiExplanationStreamHandler(IConfiguration configuration, IAiService aiService,
        IQuestionRepository questionRepository)
    {
        _configuration = configuration;
        _aiService = aiService;
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

        await foreach (var chunk in _aiService.StreamContentAsync(prompt, cancellationToken))
        {
            yield return chunk;
        }
    }
}