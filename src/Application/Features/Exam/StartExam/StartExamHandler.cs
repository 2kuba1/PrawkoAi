using Application.Contracts.Repositories;
using Application.Models;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.StartExam;

internal sealed class StartExamHandler : IRequestHandler<StartExam, StartExamResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;

    public StartExamHandler(IQuestionRepository questionRepository,
        IExamSessionRepository  examSessionRepository,
        IHttpContextAccessor httpContextAccessor,
        IExamSessionQuestionRepository  examSessionQuestionRepository)
    {
        _questionRepository = questionRepository;
        _examSessionRepository = examSessionRepository;
        _httpContextAccessor = httpContextAccessor;
        _examSessionQuestionRepository = examSessionQuestionRepository;
    }
    
    public async Task<StartExamResponse> Handle(StartExam request, CancellationToken cancellationToken)
    {
        var currentUserId = Utils.GetCurrentUserId(_httpContextAccessor);

        if (!currentUserId.HasValue || request.UserId != currentUserId.Value)
            throw new UnauthorizedAccessException();

        var examSession = new ExamSession()
        {
            UserId = currentUserId.Value,
            Id = Guid.NewGuid(),
        };
            
        await _examSessionRepository.CreateAsync(examSession);
        
        var questions = await _questionRepository.GetExamSimulationQuestions();

        await _examSessionQuestionRepository.SaveExamSessionQuestionsAsync(questions, examSession.Id);
        
        return new StartExamResponse(new ExamSessionDto(examSession.Id, examSession.StaredAt), questions);
    }
}