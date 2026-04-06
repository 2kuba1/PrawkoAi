using Application.Contracts.Repositories;
using MediatR;

namespace Application.Features.Exam.AutoFinishExam;

internal sealed class AutoFinishExamHandler : IRequestHandler<AutoFinishExam, Unit>
{
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;

    public AutoFinishExamHandler(IExamSessionRepository examSessionRepository, IExamSessionQuestionRepository examSessionQuestionRepository)
    {
        _examSessionRepository = examSessionRepository;
        _examSessionQuestionRepository = examSessionQuestionRepository;
    }
    
    public async Task<Unit> Handle(AutoFinishExam request, CancellationToken cancellationToken)
    {
        var session = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);

        if (session is null || session.FinishedAt != null)
            return Unit.Value;

        var results = await _examSessionQuestionRepository.GetScoreAndCorrectAnswerCount(session.Id);
        await _examSessionRepository.CheckIfPassedAndSaveSession(session, session.StaredAt.AddMinutes(25), (int)results.Score, results.CorrectAnswersCount);
        return Unit.Value;
    }
}