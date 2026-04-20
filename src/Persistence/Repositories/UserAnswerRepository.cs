using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Exceptions;
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

    public async Task CreateSetAnswers(Guid userId, List<UserSetAnswerDto> userSetAnswers)
    {
        if (userSetAnswers == null || userSetAnswers.Count == 0)
            return;

        var questionIds = userSetAnswers.Select(a => a.QuestionId).Distinct().ToList();
        var answerIds = userSetAnswers
            .Select(a => a.SelectedAnswerId)
            .Where(id => id != Guid.Empty) 
            .Distinct()
            .ToList();

        var existingQuestionIds = await _context.Questions
            .Where(q => questionIds.Contains(q.Id))
            .Select(q => q.Id)
            .ToListAsync();

        var existingAnswerIds = await _context.Answers
            .Where(a => answerIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync();

        var newAnswers = new List<UserAnswer>();

        foreach (var dto in userSetAnswers)
        {
            if (!existingQuestionIds.Contains(dto.QuestionId))
            {
                throw new NotFoundException($"Question with ID {dto.QuestionId} does not exist.");
            }

            if (dto.SelectedAnswerId != null)
            {
                if (!existingAnswerIds.Contains((Guid)dto.SelectedAnswerId))
                {
                    throw new NotFoundException($"Answer with ID {dto.SelectedAnswerId} does not exist.");
                }
            }

            newAnswers.Add(new UserAnswer
            {
                UserId = userId,
                QuestionId = dto.QuestionId,
                AnsweredAt = dto.AnsweredAt,
                SelectedAnswerId = dto.SelectedAnswerId
            });
        }

        await _context.UserAnswers.AddRangeAsync(newAnswers);
        await _context.SaveChangesAsync();
    }
}