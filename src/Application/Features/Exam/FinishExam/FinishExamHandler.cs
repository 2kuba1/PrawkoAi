using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.FinishExam;

internal sealed class FinishExamHandler : IRequestHandler<FinishExam, ExamResultsDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;

    public FinishExamHandler(IHttpContextAccessor httpContextAccessor, IExamSessionRepository examSessionRepository, IExamSessionQuestionRepository examSessionQuestionRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _examSessionRepository = examSessionRepository;
        _examSessionQuestionRepository = examSessionQuestionRepository;
    }
    
    public async Task<ExamResultsDto> Handle(FinishExam request, CancellationToken cancellationToken)
    {
        var isHttpContextAndRequestMatching = Utils.GetCurrentUserId(_httpContextAccessor) == request.UserId;
        
        if (!isHttpContextAndRequestMatching)
            throw new UnauthorizedException("You are not allowed to update this exam");

        var examSession = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);
        
        if(examSession is null)
            throw new NotFoundException($"Exam session with id {request.ExamSessionId} not found");
 
        if(examSession.UserId != request.UserId)
            throw new UnauthorizedException("You are not authorized to update this exam");

        if (examSession.FinishedAt is not null)
            throw new FinishedExamException("This exam session has been finished");
        
        if ((DateTime.UtcNow - examSession.StaredAt).TotalMinutes > 25)
        {
            examSession.FinishedAt ??= DateTime.UtcNow;
            throw new FinishedExamException("This exam session expired");
        }
        
        var finishedAt = DateTime.UtcNow;
        
        var results = await _examSessionQuestionRepository.GetExamResultsAsync(request.ExamSessionId);
        
        var isPassed = await _examSessionRepository.CheckIfPassedAndSaveSession(examSession, finishedAt,results.Score, results.CorrectAnswersCount);
        
        results.IsPassed = isPassed;
        results.StartedAt = examSession.StaredAt;
        results.FinishedAt = finishedAt;
        
        return results;
    }
}