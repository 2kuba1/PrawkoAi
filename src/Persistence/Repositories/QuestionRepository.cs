using Application.Contracts.Repositories;
using Application.Models;
using Application.Models.DTOs;
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
        var allIds = await _context.Questions
            .Select(q => new { q.Id, q.Points, AnsCount = q.Answers.Count })
            .ToListAsync();

        var rnd = new Random();

        var standardIds = allIds.Where(x => x.AnsCount == 2);
        var specializedIds = allIds.Where(x => x.AnsCount == 3);

        var pickedIds = new List<Guid>();
        pickedIds.AddRange(standardIds.Where(x => x.Points == 3).OrderBy(_ => rnd.Next()).Take(10).Select(x => x.Id));
        pickedIds.AddRange(standardIds.Where(x => x.Points == 2).OrderBy(_ => rnd.Next()).Take(6).Select(x => x.Id));
        pickedIds.AddRange(standardIds.Where(x => x.Points == 1).OrderBy(_ => rnd.Next()).Take(4).Select(x => x.Id));
    
        pickedIds.AddRange(specializedIds.Where(x => x.Points == 3).OrderBy(_ => rnd.Next()).Take(6).Select(x => x.Id));
        pickedIds.AddRange(specializedIds.Where(x => x.Points == 2).OrderBy(_ => rnd.Next()).Take(4).Select(x => x.Id));
        pickedIds.AddRange(specializedIds.Where(x => x.Points == 2).OrderBy(_ => rnd.Next()).Take(2).Select(x => x.Id));

        var resultQuestions = await _context.Questions
            .AsNoTracking()
            .Include(q => q.Answers)
            .Where(q => pickedIds.Contains(q.Id))
            .Select(q => new QuestionDto(
                q.Id, q.ContentPl, q.QuestionNumber, q.CategoryType, q.Points, q.MediaUrl,
                q.Answers.Select(a => new AnswerDto(a.Id, a.QuestionId, a.ContentPl, a.CreatedAt)).ToList()
            ))
            .ToListAsync(); 
        
        return new ExamQuestions(
            Standard: resultQuestions.Where(q => q.Answers.Count == 2).ToList(),
            Specialized: resultQuestions.Where(q => q.Answers.Count == 3).ToList());
    }
}