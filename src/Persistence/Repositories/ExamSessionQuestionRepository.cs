using Application.Contracts.Repositories;
using Application.Models;
using Domain.Entities;
using Persistence.Database;

namespace Persistence.Repositories;

public class ExamSessionQuestionRepository : GenericRepository<ExamSessionQuestion>, IExamSessionQuestionRepository
{
    private readonly AppDbContext _context;

    public ExamSessionQuestionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task SaveExamSessionQuestions(ExamQuestions examSessionQuestions, Guid examSessionId)
    {
        var newQuestions = new List<ExamSessionQuestion>();

        newQuestions.AddRange(examSessionQuestions.Standard.Select(q => new ExamSessionQuestion
        {
            ExamSessionId = examSessionId,
            QuestionId = q.Id
        }));

        newQuestions.AddRange(examSessionQuestions.Specialized.Select(q => new ExamSessionQuestion
        {
            ExamSessionId = examSessionId,
            QuestionId = q.Id
        }));

        await _context.ExamSessionQuestions.AddRangeAsync(newQuestions);
        await _context.SaveChangesAsync();
    }
}