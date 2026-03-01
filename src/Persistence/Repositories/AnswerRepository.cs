using Application.Contracts.Repositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
{
    private readonly AppDbContext _context;

    public AnswerRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsAnswerValid(Guid selectedAnswerId, Guid questionId)
        => await _context.Answers.AsNoTracking().AnyAsync(a => a.Id == selectedAnswerId && a.QuestionId == questionId);
}