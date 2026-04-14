using Application.Contracts.Repositories;
using Application.Contracts.Services;
using MediatR;

namespace Application.Features.Exam.AutoFinishExam;

internal sealed class AutoFinishExamHandler : IRequestHandler<AutoFinishExam, Unit>
{
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AutoFinishExamHandler(IExamSessionRepository examSessionRepository, IExamSessionQuestionRepository examSessionQuestionRepository, IUnitOfWork unitOfWork)
    {
        _examSessionRepository = examSessionRepository;
        _examSessionQuestionRepository = examSessionQuestionRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Unit> Handle(AutoFinishExam request, CancellationToken cancellationToken)
    {
        var session = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);

        if (session is null || session.FinishedAt != null)
            return Unit.Value;

        var results = await _examSessionQuestionRepository.GetScoreAndCorrectAnswerCount(session.Id);
         _examSessionRepository.CheckIfPassedAndSaveSession(session, session.StaredAt.AddMinutes(25), (int)results.Score, results.CorrectAnswersCount);
         await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}