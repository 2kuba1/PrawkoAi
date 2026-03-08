using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.GetUserExamSessionResults;

internal sealed class GetUserExamSessionResultsHandler : IRequestHandler<GetUserExamSessionResults,ExamResultsDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;
    private readonly IExamSessionRepository _examSessionRepository;

    public GetUserExamSessionResultsHandler(IHttpContextAccessor httpContextAccessor, IExamSessionQuestionRepository examSessionQuestionRepository, IExamSessionRepository examSessionRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _examSessionQuestionRepository = examSessionQuestionRepository;
        _examSessionRepository = examSessionRepository;
    }
    
    public async Task<ExamResultsDto> Handle(GetUserExamSessionResults request, CancellationToken cancellationToken)
    {
        var isHttpContextAndRequestMatching = Utils.GetCurrentUserId(_httpContextAccessor) == request.UserId;
        
        if (!isHttpContextAndRequestMatching)
            throw new UnauthorizedException("You are not allowed to update this exam");

        var examSession = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);
        
        if(examSession is null)
            throw new NotFoundException($"Exam session with id {request.ExamSessionId} not found");
 
        if(examSession.UserId != request.UserId)
            throw new UnauthorizedException("You are not authorized to update this exam");

        if (examSession.FinishedAt is null)
            throw new FinishedExamException("This exam session didn't finish");
        
        var results = await _examSessionQuestionRepository.GetExamResultsAsync(request.ExamSessionId);

        return results;
    }
}