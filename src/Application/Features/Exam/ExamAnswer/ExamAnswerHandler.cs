using Application.Contracts.Repositories;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.ExamAnswer;

public class ExamAnswerHandler : IRequestHandler<ExamAnswer, Unit>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;

    public ExamAnswerHandler(IHttpContextAccessor httpContextAccessor, IExamSessionRepository examSessionRepository, IExamSessionQuestionRepository examSessionQuestionRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _examSessionRepository = examSessionRepository;
        _examSessionQuestionRepository = examSessionQuestionRepository;
    }
    
    public async Task<Unit> Handle(ExamAnswer request, CancellationToken cancellationToken)
    {
        var isHttpContextAndRequestMatching = Utils.GetCurrentUserId(_httpContextAccessor) == request.UserId;
        
        if (!isHttpContextAndRequestMatching)
            throw new UnauthorizedException("You are not allowed to update this exam");

        var examSession = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);
        
        if(examSession is null || examSession.FinishedAt > DateTime.UtcNow)
            throw new NotFoundException("Could not find exam session or is expired");

        if(examSession.UserId != request.UserId)
            throw new UnauthorizedException("You are not allowed to update this exam");
        
        var examQuestion = await _examSessionQuestionRepository.GetByQuestionAndSessionIdAsync(request.QuestionId, request.ExamSessionId);
        
        if(examQuestion is null)
            throw new NotFoundException("Could not find exam question");
        
        await _examSessionQuestionRepository.UpdateExamSessionQuestionAsync(examQuestion, examSession.Id, request.SelectedAnswerId);
        
        return Unit.Value;
    }
}