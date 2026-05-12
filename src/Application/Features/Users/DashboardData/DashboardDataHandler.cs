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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardDataHandler(ICategoryRepository categoryRepository, IQuestionRepository questionRepository, IUserAnswerRepository userAnswerRepository, IHttpContextAccessor httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _questionRepository = questionRepository;
        _userAnswerRepository = userAnswerRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<DashboardDataDto> Handle(DashboardData request, CancellationToken cancellationToken)
    {
        if(request.UserId != Utils.GetCurrentUserId(_httpContextAccessor))
            throw new UnauthorizedException("You are not authorized to revoke this refresh tokens");
        
        var worstPerformingCategory = await _categoryRepository.GetUserWorsePerformingCategory(request.UserId);
        var uniqueQuestionsAnswered = await _userAnswerRepository.GetUniqueQuestionsAnsweredCount(request.UserId, request.Category);
        var getQuestionsOfCategory = await _questionRepository.GetQuestionsCountOfCategory(request.Category);
        var todayQuestionsAnswered= await _userAnswerRepository.TodayQuestionsAnsweredCount(request.UserId);
       
        throw new NotImplementedException();
    }
}