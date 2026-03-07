using Application.Contracts.Repositories;
using Application.Models;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Exceptions;
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
                           ?? throw new InvalidQuestionException("Invalid question");

        var examQuestion = await _context.ExamSessionQuestions
                               .FirstOrDefaultAsync(e => e.Id == updateDto.Id && e.ExamSessionId == examSessionId)
                           ?? throw new InvalidExamSessionOrQuestionException("Invalid exam session or question");

        if(examQuestion.SelectedAnswerId is not null)
            throw new  AnswerAlreadyAddedException("You have already answered this question!");
        
        examQuestion.AnsweredAt = DateTime.UtcNow;
        examQuestion.SelectedAnswerId = selectedAnswerId;
        examQuestion.IsCorrect = selectedAnswerId == questionData.CorrectAnswerId;

        await _context.SaveChangesAsync();
    }

    public async Task<ExamSessionQuestion?> GetByQuestionAndSessionIdAsync(Guid questionId, Guid examSessionId)
        => await _context.ExamSessionQuestions.FirstOrDefaultAsync(q => q.QuestionId == questionId && q.ExamSessionId == examSessionId);

    public async Task<ExamResultsDto> GetExamResultsAsync(Guid examSessionId)
    {
        var sessionData = await _context.ExamSessions
            .AsNoTracking()
            .Where(s => s.Id == examSessionId)
            .Select(s => new {
                s.StaredAt,
                Questions = s.ExamSessionQuestions.Select(q => new {
                    q.Id,
                    q.QuestionId,
                    q.SelectedAnswerId,
                    q.IsCorrect,
                    q.AnsweredAt,
                    QuestionContent = q.Question.Content,
                    QuestionPoints = q.Question.Points 
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (sessionData == null) throw new NotFoundException("Session doesn't exist");
        

        var results = sessionData.Questions
            .GroupBy(q => q.IsCorrect == true)
            .ToDictionary(g => g.Key, g => g.ToList());
    
        var correct = results.GetValueOrDefault(true, new());
        var incorrect = results.GetValueOrDefault(false, new());

        var totalScore = correct.Sum(x => x.QuestionPoints);

        return new ExamResultsDto
        {
            CorrectAnswersCount = correct.Count,
            CorrectAnswers =  correct.Select(x => new AnswerDto(x.Id, x.QuestionId, x.QuestionContent,
            x.AnsweredAt ?? DateTime.UtcNow)).ToList(),
            NotCorrectAnswers = incorrect.Select(x => new AnswerDto(x.Id, x.QuestionId, x.QuestionContent,
            x.AnsweredAt ?? DateTime.UtcNow)).ToList(),
            Score =  (int)totalScore,
        };
    }
}