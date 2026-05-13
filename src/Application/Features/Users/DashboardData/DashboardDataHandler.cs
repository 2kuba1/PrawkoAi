using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.DashboardData;

internal sealed class DashboardDataHandler : IRequestHandler<DashboardData, DashboardDataDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardDataHandler(ICategoryRepository categoryRepository, IQuestionRepository questionRepository, IUserAnswerRepository userAnswerRepository, IExamSessionRepository examSessionRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _questionRepository = questionRepository;
        _userAnswerRepository = userAnswerRepository;
        _examSessionRepository = examSessionRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<DashboardDataDto> Handle(DashboardData request, CancellationToken cancellationToken)
    {
        if(request.UserId != Utils.GetCurrentUserId(_httpContextAccessor))
            throw new UnauthorizedException("You are not authorized to revoke this refresh tokens");
        
        var worstPerformingCategory = _categoryRepository.GetUserWorsePerformingCategory(request.UserId);
        var uniqueQuestionsAnswered = _userAnswerRepository.GetUniqueQuestionsAnsweredCount(request.UserId, request.Category);
        var questionsOfCategory = _questionRepository.GetQuestionsCountOfCategory(request.Category);
        var todayQuestionsAnswered= _userAnswerRepository.TodayQuestionsAnsweredCount(request.UserId);
        var avgExamScore = _examSessionRepository.GetAverageExamScore(request.UserId);
        var streak = _userRepository.GetStreak(request.UserId);

        await Task.WhenAll(worstPerformingCategory, uniqueQuestionsAnswered, questionsOfCategory, avgExamScore, todayQuestionsAnswered, streak);
        
        return new DashboardDataDto(
            worstPerformingCategory.Result,
            questionsOfCategory.Result,
            uniqueQuestionsAnswered.Result,
            streak.Result,
            avgExamScore.Result,
            streak.Result);
    }
}