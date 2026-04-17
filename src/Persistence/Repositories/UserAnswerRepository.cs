using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class UserAnswerRepository : GenericRepository<UserAnswer>, IUserAnswerRepository
{
    private readonly AppDbContext _context;

    public UserAnswerRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<UserLastAnswersDto>> GetUserLastAnswers(Guid userId)
    {
        return await _context.UserAnswers
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserLastAnswersDto(
                u.Question.CategoryTag,
                u.AnsweredAt,
                u.SelectedAnswerId == u.Question.CorrectAnswerId,
                u.Question.Points
            ))
            .ToListAsync();
    }
}