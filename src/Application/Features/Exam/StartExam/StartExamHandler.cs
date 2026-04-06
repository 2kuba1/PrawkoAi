using Application.Contracts.Repositories;
using Application.Jobs;
using Application.Models;
using Application.Models.DTOs;
using Application.Shared;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Quartz;

namespace Application.Features.Exam.StartExam;

internal sealed class StartExamHandler : IRequestHandler<StartExam, StartExamResponse>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;
    private readonly ISchedulerFactory _schedulerFactory;

    public StartExamHandler(IQuestionRepository questionRepository,
        IExamSessionRepository  examSessionRepository,
        IHttpContextAccessor httpContextAccessor,
        IExamSessionQuestionRepository  examSessionQuestionRepository,
        ISchedulerFactory schedulerFactory)
    {
        _questionRepository = questionRepository;
        _examSessionRepository = examSessionRepository;
        _httpContextAccessor = httpContextAccessor;
        _examSessionQuestionRepository = examSessionQuestionRepository;
        _schedulerFactory = schedulerFactory;
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
        
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var job = JobBuilder.Create<AutoFinishExamJob>()
            .WithIdentity($"Job-{examSession.Id}")
            .UsingJobData("ExamSessionId", examSession.Id)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"Trigger-{examSession.Id}")
            .StartAt(DateTimeOffset.UtcNow.AddMinutes(1))
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
        
        var questions = await _questionRepository.GetExamSimulationQuestions(request.Category, request.Locale);

        await _examSessionQuestionRepository.SaveExamSessionQuestionsAsync(questions, examSession.Id);
        
        return new StartExamResponse(new ExamSessionDto(examSession.Id, examSession.StaredAt), questions);
    }
}