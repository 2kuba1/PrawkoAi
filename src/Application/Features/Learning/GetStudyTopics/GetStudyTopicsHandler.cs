using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Learning.GetStudyTopics;

internal sealed class GetStudyTopicsHandler : IRequestHandler<GetStudyTopics, List<GetStudyTopicsResponeDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStudyTopicsHandler(IQuestionRepository questionRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _questionRepository = questionRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<List<GetStudyTopicsResponeDto>> Handle(GetStudyTopics request, CancellationToken cancellationToken)
    {
        if (request.UserId != Utils.GetCurrentUserId(_httpContextAccessor)!.Value)
            throw new UnauthorizedException("You can get this data");
    
        await _userRepository.UpdateStreak(request.UserId);
        
        var results  = await _questionRepository.GetUserLearningProgressAndTopicsCount(request.UserId, request.Category.ToUpper());
    
        return results;
    }
}