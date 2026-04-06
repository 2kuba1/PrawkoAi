using Application.Features.Exam.AutoFinishExam;
using MediatR;
using Quartz;

namespace Application.Jobs;

public class AutoFinishExamJob : IJob
{
    private readonly IMediator _mediator;

    public AutoFinishExamJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var sessionId = dataMap.GetGuid("ExamSessionId");

        await _mediator.Send(new AutoFinishExam(sessionId));
    }
}