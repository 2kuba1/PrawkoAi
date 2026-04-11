using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Domain.Enums;
using MathNet.Numerics.Distributions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Users.GetStats;

internal sealed class GetStatsHandler : IRequestHandler<GetStats, UserStatsDto>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IUserAiProgressRepository _aiProgressRepository;
    private readonly IConfiguration _config;

    public GetStatsHandler(IUserAnswerRepository userAnswerRepository, IExamSessionRepository examSessionRepository, IUserAiProgressRepository aiProgressRepository,IConfiguration config)
    {
        _userAnswerRepository = userAnswerRepository;
        _examSessionRepository = examSessionRepository;
        _aiProgressRepository = aiProgressRepository;
        _config = config;
    }
    
    public async Task<UserStatsDto> Handle(GetStats request, CancellationToken cancellationToken)
    {
        var answersCount = _config.GetValue<int>("Ai:AnswersCount");
        var lastAnswers = await _userAnswerRepository.GetUserLastAnswers(request.UserId, answersCount);
        var lastExamsScores = await _examSessionRepository.GetLastExamsScores(request.UserId, 10);

        var avgScore = lastExamsScores.Count != 0 ? lastExamsScores.Average() ?? 0.0 : 0.0;
        var trend = lastExamsScores.Count >= 2 ? lastExamsScores.First() - lastExamsScores.Last() : 0;
        
        var answersStats = lastAnswers
            .GroupBy(x => x.CategoryTag)
            .Select(g => new AnswersStatsDto(
                g.Key,
                Math.Round((double)g.Count(x => x.WasCorrectlyAnswered) / g.Count() * 100, 1),
                g.Count(),
                g.Count(x => x is { WasCorrectlyAnswered: false, Points: 1 }),
                g.Count(x => x is { WasCorrectlyAnswered: false, Points: 2 }),
                g.Count(x => x is { WasCorrectlyAnswered: false, Points: 3 })
            ))
            .ToList();
        
        var examStats =new {
            AverageScore = avgScore,
            MaxScore = 74,
            PassThreshold = 68,
            ExamTrend = trend > 0 ? ExamTrend.Upward : ExamTrend.Downward,
        };
        
        var totalAccuracy = lastAnswers.Count != 0 
            ? (Math.Round((double)lastAnswers.Count(x => x.WasCorrectlyAnswered) / lastAnswers.Count * 100)) 
            : 0;
        
        var sumOfSquares = lastExamsScores.Select(s => Math.Pow(avgScore, 2)).Sum();
        var standardDeviation = Math.Sqrt(sumOfSquares / lastExamsScores.Count);
        var dist = new Normal(avgScore, standardDeviation);
        var probOfFailure = dist.CumulativeDistribution(68);
        var passProbability = Math.Round((1 - probOfFailure) * 100);

        var userAiProgressContent = await _aiProgressRepository.GetAiProgress(request.UserId);
        
        return new UserStatsDto(
            answersStats,
            examStats.AverageScore,
            examStats.ExamTrend,
            passProbability,
            totalAccuracy,
            userAiProgressContent
        );
    }
}