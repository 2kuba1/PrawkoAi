using Application.Common;
using Application.Contracts.Repositories;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Features.Answers.LogUserAnswersInSet;

internal sealed class LogUserAnswersInSetHandler : IRequestHandler<LogUserAnswersInSet, Unit>
{
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDistributedCache _cache;

    public LogUserAnswersInSetHandler(IUserAnswerRepository userAnswerRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
    {
        _userAnswerRepository = userAnswerRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }
    
    public async Task<Unit> Handle(LogUserAnswersInSet request, CancellationToken cancellationToken)
    {
        var isHttpContextAndRequestMatching = Utils.GetCurrentUserId(_httpContextAccessor) == request.UserId;
        
        if (!isHttpContextAndRequestMatching)
            throw new UnauthorizedException("You are not allowed to write this answers to this user");

        await _userRepository.UpdateStreak(request.UserId);
        await _userAnswerRepository.CreateSetAnswers(request.UserId, request.Answers);
        
        var cacheKey = $"user_stats_{request.UserId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        
        return Unit.Value;
    }
}