using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Enums;
using Domain.Exceptions;
using MathNet.Numerics.Distributions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Users.GetStats;

internal sealed class GetStatsHandler : IRequestHandler<GetStats, UserStatsDto>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IUserAiProgressRepository _aiProgressRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStatsHandler(IUserAnswerRepository userAnswerRepository, IExamSessionRepository examSessionRepository, IUserAiProgressRepository aiProgressRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userAnswerRepository = userAnswerRepository;
        _examSessionRepository = examSessionRepository;
        _aiProgressRepository = aiProgressRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserStatsDto> Handle(GetStats request, CancellationToken cancellationToken)
    {
        if(request.UserId != Utils.GetCurrentUserId(_httpContextAccessor))
            throw new UnauthorizedException("You are not authorized to revoke this refresh tokens");
        
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

        var passProbability = Utils.CalculatePassProbability(lastExamsScores, answersStats);

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