using Application.Contracts.Repositories;
using Application.Models;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class ExamSessionQuestionRepository : GenericRepository<ExamSessionQuestion>, IExamSessionQuestionRepository
{
    private readonly AppDbContext _context;

    public ExamSessionQuestionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task SaveExamSessionQuestionsAsync(ExamQuestions examSessionQuestions, Guid examSessionId)
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

    public async Task UpdateExamSessionQuestionAsync(ExamSessionQuestion updateDto, Guid examSessionId, Guid selectedAnswerId)
    {
        var questionData = await _context.Questions
                               .Where(q => q.Id == updateDto.QuestionId)
                               .Select(q => new { q.CorrectAnswerId })
                               .FirstOrDefaultAsync() 
                           ?? throw new ApplicationException("Invalid question");

        var examQuestion = await _context.ExamSessionQuestions
                               .FirstOrDefaultAsync(e => e.Id == updateDto.Id && e.ExamSessionId == examSessionId)
                           ?? throw new ApplicationException("Invalid exam session or question");

        if(examQuestion.SelectedAnswerId is not null)
            throw new  ApplicationException("You have already answered this question!");
        
        examQuestion.AnsweredAt = DateTime.UtcNow;
        examQuestion.SelectedAnswerId = selectedAnswerId;
        examQuestion.IsCorrect = selectedAnswerId == questionData.CorrectAnswerId;

        await _context.SaveChangesAsync();
    }

    public async Task<ExamSessionQuestion?> GetByQuestionAndSessionIdAsync(Guid questionId, Guid examSessionId)
        => await _context.ExamSessionQuestions.FirstOrDefaultAsync(q => q.QuestionId == questionId && q.ExamSessionId == examSessionId);
    
}