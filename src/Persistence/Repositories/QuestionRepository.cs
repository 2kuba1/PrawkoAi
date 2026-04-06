using Application.Contracts.Repositories;
using Application.Models;
using Application.Models.DTOs;
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

    public async Task<ExamQuestions> GetExamSimulationQuestions(string category, string? locale)
    {
        var allIds = await _context.Questions
            .Where(q => q.Categories.Any(c => c.Name == category))
            .Select(q => new { q.Id, q.Points, AnsCount = q.Answers.Count })
            .ToListAsync();

        var rnd = new Random();
        var standardIds = allIds.Where(x => x.AnsCount == 2).ToList();
        var specializedIds = allIds.Where(x => x.AnsCount == 3).ToList();

        var pickedIds = new List<Guid>();
        void Pick(List<Guid> target, IEnumerable<Guid> source, int count) =>
            target.AddRange(source.OrderBy(_ => rnd.Next()).Take(count));

        Pick(pickedIds, standardIds.Where(x => x.Points == 3).Select(x => x.Id), 10);
        Pick(pickedIds, standardIds.Where(x => x.Points == 2).Select(x => x.Id), 6);
        Pick(pickedIds, standardIds.Where(x => x.Points == 1).Select(x => x.Id), 4);

        Pick(pickedIds, specializedIds.Where(x => x.Points == 3).Select(x => x.Id), 6);
        Pick(pickedIds, specializedIds.Where(x => x.Points == 2).Select(x => x.Id), 4);
        Pick(pickedIds, specializedIds.Where(x => x.Points == 1).Select(x => x.Id), 2);

        var lang = locale?.ToUpper() ?? "PL";

        var resultQuestions = await _context.Questions
            .AsNoTracking()
            .Where(q => pickedIds.Contains(q.Id))
            .Select(q => new QuestionDto(
                q.Id,
                (lang == "PL" ? q.ContentPl :
                    lang == "EN" ? q.ContentEn :
                    lang == "DE" ? q.ContentDe :
                    lang == "UA" ? (string.IsNullOrEmpty(q.ContentUa) ? q.ContentPl : q.ContentUa) : 
                    q.ContentEn)!,
                q.QuestionNumber,
                q.CategoryType,
                q.Points,
                q.MediaUrl,
                q.Answers.Select(a => new AnswerDto(
                    a.Id,
                    a.QuestionId,
                    (lang == "PL" ? a.ContentPl :
                        lang == "EN" ? a.ContentEn :
                        lang == "DE" ? a.ContentDe :
                        lang == "UA" ? (string.IsNullOrEmpty(a.ContentUa) ? a.ContentPl : a.ContentUa) : 
                        a.ContentEn)!,
                    a.CreatedAt
                )).ToList()
            ))
            .ToListAsync();

        return new ExamQuestions(
            Standard: resultQuestions.Where(q => q.Answers.Count == 2).OrderBy(_ => rnd.Next()).ToList(),
            Specialized: resultQuestions.Where(q => q.Answers.Count == 3).OrderBy(_ => rnd.Next()).ToList()
        );
    }

    public Task<GetQuestionAdditionalDataDto?> GetQuestionAdditionalData(Guid questionId, string locale)
    {
        var lang =  locale.ToUpper();

        var mediaAndStaticResponse = _context.Questions.Where(q => q.Id == questionId)
            .Select(q => new GetQuestionAdditionalDataDto
            (
                q.MediaUrl,
                lang == "PL" ? q.StaticResponsePl : lang == "EN"  ? q.StaticResponseEn : lang == "DE" ? q.StaticResponseDe :
                    lang == "UA" ? (string.IsNullOrEmpty(q.StaticResponseUa) ?  q.StaticResponsePl : q.StaticResponseUa) : q.StaticResponseEn,
                q.CorrectAnswer!.Id
            ))
            .FirstOrDefaultAsync();
        
        return mediaAndStaticResponse;
    }
}