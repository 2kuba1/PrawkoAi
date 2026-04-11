using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        bool isPassed = score >= 68;
        examSession.IsPassed = isPassed;
        examSession.Score = score;
        examSession.FinishedAt = finishedAt;
        examSession.CorrectAnswersCount = correctAnswerCount;
        _context.ExamSessions.Update(examSession);
        await _context.SaveChangesAsync();
        return isPassed;
    }

    public async Task<List<ExamSessionHistory>> GetUserExamsSessionHistory(Guid userId)
    {
        return await _context.ExamSessions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.FinishedAt != null)
            .Select(x => new ExamSessionHistory(
                userId,
                x.Id,
                x.StaredAt,
                x.FinishedAt,
                x.Score,
                x.IsPassed,
                x.CorrectAnswersCount))
            .ToListAsync();
    }

    public async Task<List<int?>> GetLastExamsScores(Guid userId, int examsCount) 
        =>await _context.ExamSessions
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(examsCount)
            .Select(e => e.Score)
            .ToListAsync();
}