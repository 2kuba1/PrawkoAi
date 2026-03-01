using Application.Contracts.Repositories;
using Application.Models;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    private readonly AppDbContext _context;

    public QuestionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Question?> GetRandomQuestionByCategory(Guid categoryId)
    {
        var query = _context.Questions
            .Where(x => x.Categories.Any(c => c.Id == categoryId));

        var questionIds = await query
            .Select(x => x.Id)
            .ToListAsync();

        if (questionIds.Count == 0)
            return null;

        var randomId = questionIds[new Random().Next(questionIds.Count)];

        return await _context.Questions
            .AsNoTracking()
            .Where(x => x.Id == randomId)
            .Include(x => x.Answers)
            .Include(x => x.CorrectAnswer)
            .Include(x => x.Categories.Where(c => c.Id == categoryId))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfQuestionExists(Guid questionId)
        => await _context.Questions.AsNoTracking().AnyAsync(x => x.Id == questionId);

    public async Task<ExamQuestions> GetExamSimulationQuestions()
    {
        var standardQuestionsWith3PointsIds = _context.Questions.Where(q => q.Points == 3 && q.Answers.Count == 2).OrderBy(q => EF.Functions.Random()).Select(q => q.Id).Take(10);
        var standardQuestionsWith2PointsIds = _context.Questions.Where(q => q.Points == 2 && q.Answers.Count == 2).OrderBy(q => EF.Functions.Random()).Select(q => q.Id).Take(6);
        var standardQuestionsWith1PointIds = _context.Questions.Where(q => q.Points == 1 && q.Answers.Count == 2).OrderBy(q => EF.Functions.Random()).Select(q => q.Id).Take(4);

        var specializedQuestionsWith3PointsIds = _context.Questions.Where(q => q.Points == 3 && q.Answers.Count == 3).OrderBy(q => EF.Functions.Random()).Select(q => q.Id).Take(6);
        var specializedQuestionsWith2PointsIds = _context.Questions.Where(q => q.Points == 2 && q.Answers.Count == 3).OrderBy(q => EF.Functions.Random()).Select(q => q.Id).Take(4);
        var specializedQuestionsWith1PointIds = _context.Questions.Where(q => q.Points == 1 && q.Answers.Count == 3).OrderBy(q => EF.Functions.Random()).Select(q => q.Id).Take(2);

    
        var allStandardIds = await standardQuestionsWith3PointsIds.Concat(standardQuestionsWith2PointsIds).Concat(standardQuestionsWith1PointIds).ToListAsync();
        var allSpecializedIds = await specializedQuestionsWith3PointsIds.Concat(specializedQuestionsWith2PointsIds).Concat(specializedQuestionsWith1PointIds).ToListAsync();

        var standardQuestions = await _context.Questions
            .AsNoTracking()
            .Include(q => q.Answers)
            .Where(q => allStandardIds.Contains(q.Id))
            .ToListAsync();

        var specializedQuestions = await _context.Questions
            .AsNoTracking()
            .Include(q => q.Answers)
            .Where(q => allSpecializedIds.Contains(q.Id))
            .ToListAsync();

        var random = new Random();
    
        return new ExamQuestions(
            Standard: standardQuestions.OrderBy(_ => random.Next()).ToList(),
            Specialized: specializedQuestions.OrderBy(_ => random.Next()).ToList()
        );
    }
}