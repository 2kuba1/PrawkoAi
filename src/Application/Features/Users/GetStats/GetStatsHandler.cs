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

    public GetStatsHandler(IUserAnswerRepository userAnswerRepository, IExamSessionRepository examSessionRepository, IUserAiProgressRepository aiProgressRepository, IConfiguration config)
    {
        _userAnswerRepository = userAnswerRepository;
        _examSessionRepository = examSessionRepository;
        _aiProgressRepository = aiProgressRepository;
        _config = config;
    }

    public async Task<UserStatsDto> Handle(GetStats request, CancellationToken cancellationToken)
    {
        var lastAnswers = await _userAnswerRepository.GetUserLastAnswers(request.UserId);
        var lastExamsScoresNullable = await _examSessionRepository.GetLastExamsScores(request.UserId);
        
        var lastExamsScores = lastExamsScoresNullable.Where(x => x.HasValue).Select(x => (double)x!.Value).ToList();

        var avgScore = lastExamsScores.Count != 0 ? lastExamsScores.Average() : 0.0;
        var trendValue = lastExamsScores.Count >= 2 ? lastExamsScores.First() - lastExamsScores.Last() : 0;

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

        var examTrend = trendValue >= 0 ? ExamTrend.Upward : ExamTrend.Downward;

        var totalAccuracy = lastAnswers.Count != 0
            ? (Math.Round((double)lastAnswers.Count(x => x.WasCorrectlyAnswered) / lastAnswers.Count * 100))
            : 0;

        double passProbability = 0;

        switch (lastExamsScores.Count)
        {
            case >= 2:
            {
                var sumOfSquares = lastExamsScores.Select(s => Math.Pow(s - avgScore, 2)).Sum();
                var standardDeviation = Math.Sqrt(sumOfSquares / lastExamsScores.Count);

                if (standardDeviation > 0)
                {
                    var dist = new Normal(avgScore, standardDeviation);
                    var probOfFailure = dist.CumulativeDistribution(68);
                    passProbability = Math.Round((1 - probOfFailure) * 100);
                }
                else
                {
                    passProbability = avgScore >= 68 ? 100 : 0;
                }

                break;
            }
            case 1:
                passProbability = lastExamsScores[0] >= 68 ? 100 : 0;
                break;
        }

        passProbability = Math.Clamp(passProbability, 0, 100);

        var userAiProgressContent = await _aiProgressRepository.GetAiProgress(request.UserId);

        return new UserStatsDto(
            answersStats,
            avgScore,
            examTrend,
            passProbability,
            totalAccuracy,
            userAiProgressContent
        );
    }
}