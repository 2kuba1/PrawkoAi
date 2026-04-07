using System.Net.Http.Json;
using System.Text.Json;
using Application.Contracts.Repositories;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.AI.AnalyzeUserProgress;

internal sealed class AnalyzeUserProgressHandler : IRequestHandler<AnalyzeUserProgress, string>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public AnalyzeUserProgressHandler(IUserAnswerRepository userAnswerRepository, IExamSessionRepository examSessionRepository, IHttpClientFactory httpClientFactory,IConfiguration config)
    {
        _userAnswerRepository = userAnswerRepository;
        _examSessionRepository = examSessionRepository;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }
    
    public async Task<string> Handle(AnalyzeUserProgress request, CancellationToken cancellationToken)
    {
        var answersCount = _config.GetValue<int>("Ai:AnswersCount");
        var lastAnswers = await _userAnswerRepository.GetUserLastAnswers(request.UserId, answersCount);
        var lastExamsScores = await _examSessionRepository.GetLastExamsScores(request.UserId, 5);
        
        var avgScore = lastExamsScores.Count != 0 ? lastExamsScores.Average() : 0;
        var trend = lastExamsScores.Count >= 2 ? lastExamsScores.First() - lastExamsScores.Last() : 0;
        
        var answersStats = lastAnswers
            .GroupBy(x => x.CategoryTag)
            .Select(g => new {
                Category = g.Key,
                Accuracy = Math.Round((double)g.Count(x => x.WasCorrectlyAnswered) / g.Count() * 100, 1),
                TotalAttempts = g.Count(),
                SmallErrors = g.Count(x => x is { WasCorrectlyAnswered: false, Points: 1 }),
                MediumErrors = g.Count(x => x is { WasCorrectlyAnswered: false, Points: 2 }),
                CriticalErrors = g.Count(x => x is { WasCorrectlyAnswered: false, Points: 3 })
            })
            .ToList();
        
        var examStats =new {
            AverageScore = avgScore,
            MaxScore = 74,
            PassThreshold = 68,
            ExamTrend = trend > 0 ? "Wzrostowy" : "Spadkowy",
        };

        var prompt = $"""
                       Jesteś inteligentnym intruktorem nauki jazdy. (to jest nauka na teorię więc osoba nie ma doświadczenia za kierownicą)
                       Dane kursanta:
                       - Średni wynik z egzaminów: {examStats.AverageScore}/74 (Próg zaliczenia: 68).
                       - Trend wyników: {examStats.ExamTrend}.
                       - Statystyki kategorii: {JsonSerializer.Serialize(answersStats)}.
                       
                       Twoje zadanie:
                       1. Szansa na zdanie (%) - podaj tylko liczbę i krótkie uzasadnienie.
                       2. Główny problem - max 1 zdanie.
                       3. Mocna strona - max 1 zdanie.
                       4. Złota rada - max 1 zdanie.
                       
                       STRIKTNE WYMAGANIA: 
                       - Odpowiadaj bardzo zwięźle. 
                       - Całość MUSI zamknąć się w maksymalnie 100 słowach.
                       - Jeśli będziesz zbyt wylewny, Twoja odpowiedź zostanie ucięta.
                       
                       Odpowiadaj pełnym zdaniem, nie numeruj odpowiedzi tylko sklej to wszystko logicznie
                       
                       NIE UŻYWAJ MARKDOWN, POGRUBIEN, LIST ITP.
                       """;
        
        var apiKey = _config["AI:GeminiApiKey"];
        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:generateContent?key={apiKey}";
        
        var payload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { 
                temperature = 0.5, 
                maxOutputTokens = 400,
                topP = 0.8
            }
        };

        using var client = _httpClientFactory.CreateClient();
        
        client.Timeout = TimeSpan.FromSeconds(12);
        
        try 
        {
            var response = await client.PostAsJsonAsync(url, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return "Analiza jest chwilowo niedostępna. Spróbuj później.";

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);
            var aiText = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            return aiText?.Trim() ?? "Analiza jest chwilowo niedostępna.";
        }
        catch (OperationCanceledException)
        {
            return "Czas oczekiwania na analizę został przekroczony.";
        }
    }
}