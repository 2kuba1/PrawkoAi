using Application.Contracts.Repositories;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Answers.LogUserAnswersInSet;

internal sealed class LogUserAnswersInSetHandler : IRequestHandler<LogUserAnswersInSet, Unit>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogUserAnswersInSetHandler(IUserAnswerRepository userAnswerRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userAnswerRepository = userAnswerRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<Unit> Handle(LogUserAnswersInSet request, CancellationToken cancellationToken)
    {
        var isHttpContextAndRequestMatching = Utils.GetCurrentUserId(_httpContextAccessor) == request.UserId;
        
        if (!isHttpContextAndRequestMatching)
            throw new UnauthorizedException("You are not allowed to write this answers to this user");

        await _userRepository.UpdateStreak(request.UserId);
        await _userAnswerRepository.CreateSetAnswers(request.UserId, request.Answers);
        
        return Unit.Value;
    }
}