using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.FinishExam;

public class FinishExamHandler : IRequestHandler<FinishExam, ExamResultsDto>
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
            throw new UnauthorizedAccessException("You are not allowed to update this exam");

        var examSession = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);
        
        if(examSession is null)
            throw new ApplicationException($"Exam session with id {request.ExamSessionId} not found");
 
        if(examSession.UserId != request.UserId)
            throw new UnauthorizedAccessException("You are not authorized to update this exam");

        if ((DateTime.UtcNow - examSession.StaredAt).TotalMinutes > 32)
        {
            examSession.FinishedAt ??= DateTime.UtcNow;
            throw new ApplicationException("This exam session expired");
        }
        
        var results = await _examSessionQuestionRepository.GetExamResultsAsync(request.ExamSessionId);

        return results;
    }
}