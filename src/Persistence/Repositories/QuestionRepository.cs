using Application.Contracts.Repositories;
using Domain;
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
            .Include(x => x.CorrectAnswer)
            .Include(x => x.Categories.Where(c => c.Id == categoryId))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfQuestionExists(Guid questionId)
        => await _context.Questions.AsNoTracking().AnyAsync(x => x.Id == questionId);
}