using Application.Contracts.Repositories;
using Application.Models;
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

    public bool CheckIfPassedAndSaveSession(ExamSession examSession, DateTime finishedAt,int score, int correctAnswerCount)
    {
        bool isPassed = score >= 68;
        examSession.IsPassed = isPassed;
        examSession.Score = score;
        examSession.FinishedAt = finishedAt;
        examSession.CorrectAnswersCount = correctAnswerCount;
        _context.ExamSessions.Update(examSession);
        return isPassed;
    }

    public async Task<PagedList<ExamSessionHistory>> GetUserExamsSessionHistory(Guid userId, int pageNumber, int pageSize)
    {
        var query = _context.ExamSessions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.FinishedAt != null);

        var totalCount = await query.CountAsync();
    
        var items = await query
            .OrderByDescending(x => x.FinishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ExamSessionHistory(
                userId,
                x.Id,
                x.StaredAt,
                x.FinishedAt,
                x.Score,
                x.IsPassed,
                x.CorrectAnswersCount))
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedList<ExamSessionHistory>(items, pageNumber, totalCount, totalPages);
    }

    public async Task<List<int?>> GetLastExamsScores(Guid userId) 
        =>await _context.ExamSessions
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => e.Score)
            .ToListAsync();

    public async Task<float> GetAverageExamScore(Guid userId)
        => await _context.ExamSessions
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.FinishedAt != null)
            .AverageAsync(e => (float?)e.Score) ?? 0f;
}