using Application.Contracts.Repositories;
using Domain.Entities;
using Persistence.Database;

namespace Persistence.Repositories;

public class ExamSessionRepository : GenericRepository<ExamSession>, IExamSessionRepository
{
    private readonly AppDbContext _context;

    public ExamSessionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> CheckIfPassedAndSaveSession(ExamSession examSession, DateTime finishedAt,int score, int correctAnswerCount)
    {
        bool isPassed = score >= 68 ? true : false;
        examSession.IsPassed = isPassed;
        examSession.Score = score;
        examSession.FinishedAt = finishedAt;
        _context.ExamSessions.Update(examSession);
        await _context.SaveChangesAsync();
        return isPassed;
    }
}