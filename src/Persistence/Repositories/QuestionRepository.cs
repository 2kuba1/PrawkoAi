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
        var lang = locale?.ToUpper() ?? "PL";

        var pickedIds = new List<Guid>();

        pickedIds.AddRange(await GetRandomIds(2, 3, 10));
        pickedIds.AddRange(await GetRandomIds(2, 2, 6));
        pickedIds.AddRange(await GetRandomIds(2, 1, 4));

        pickedIds.AddRange(await GetRandomIds(3, 3, 6));
        pickedIds.AddRange(await GetRandomIds(3, 2, 4));
        pickedIds.AddRange(await GetRandomIds(3, 1, 2));

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
            Standard: resultQuestions.Where(q => q.Answers.Count == 2).ToList(),
            Specialized: resultQuestions.Where(q => q.Answers.Count == 3).ToList()
        );

        async Task<List<Guid>> GetRandomIds(int ansCount, int points, int take)
        {
            return await _context.Questions
                .Where(q => q.Categories.Any(c => c.Name == category)
                            && q.Answers.Count == ansCount
                            && q.Points == points)
                .OrderBy(q => Guid.NewGuid())
                .Select(q => q.Id)
                .Take(take)
                .ToListAsync();
        }
    }

    public Task<GetQuestionAdditionalDataDto?> GetQuestionAdditionalData(Guid questionId, string locale)
    {
        var lang = locale.ToUpper();

        var mediaAndStaticResponse = _context.Questions.Where(q => q.Id == questionId)
            .Select(q => new GetQuestionAdditionalDataDto
            (
                q.MediaUrl,
                lang == "PL" ? q.StaticResponsePl :
                lang == "EN" ? q.StaticResponseEn :
                lang == "DE" ? q.StaticResponseDe :
                lang == "UA" ? (string.IsNullOrEmpty(q.StaticResponseUa) ? q.StaticResponsePl : q.StaticResponseUa) :
                q.StaticResponseEn,
                q.CorrectAnswer!.Id
            ))
            .FirstOrDefaultAsync();

        return mediaAndStaticResponse;
    }

    public async Task<AiRequiredDataDto?> GetRequiredAiData(Guid questionId, string locale)
    {
        var lang = locale.ToUpper();

        var result = await _context.Questions.Where(q => q.Id == questionId)
            .Select(q => new AiRequiredDataDto(
                q.AiContext,
                lang == "PL" ? q.StaticResponsePl :
                lang == "EN" ? q.StaticResponseEn :
                lang == "DE" ? q.StaticResponseDe :
                lang == "UA" ? (string.IsNullOrEmpty(q.StaticResponseUa) ? q.StaticResponsePl : q.StaticResponseUa) :
                q.StaticResponseEn,
                lang == "PL" ? q.ContentPl :
                lang == "EN" ? q.ContentEn :
                lang == "DE" ? q.ContentDe :
                lang == "UA" ? (string.IsNullOrEmpty(q.ContentUa) ? q.ContentPl : q.ContentUa) :
                q.ContentEn
            )).FirstOrDefaultAsync();

        return result;
    }

    public async Task<List<GetStudyTopicsResponeDto>> GetUserLearningProgressAndTopicsCount(Guid userId, string category)
    {
        var categoriesTags = await _context.Questions
            .AsNoTracking()
            .Where(q => q.Categories.Any(c => c.Name == category))
            .Select(q => q.CategoryTag)
            .Distinct()
            .ToListAsync();

        var allQuestionsCounts = await _context.Questions
            .AsNoTracking()
            .Where(q => q.Categories.Any(c => c.Name == category))
            .GroupBy(q => q.CategoryTag)
            .Select(g => new { Tag = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Tag!, x => x.Count);

        var userProgressCounts = await _context.UserAnswers
            .AsNoTracking()
            .Where(u => u.UserId == userId && u.Question!.Categories.Any(c => c.Name == category))
            .Select(u => new { u.QuestionId, u.Question!.CategoryTag })
            .Distinct() 
            .GroupBy(x => x.CategoryTag)
            .Select(g => new { Tag = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Tag!, x => x.Count);

        var studyTopicsResponse = categoriesTags
            .Where(tag => tag != null)
            .Select(tag => new GetStudyTopicsResponeDto(
                tag!,
                allQuestionsCounts.GetValueOrDefault(tag!, 0),
                userProgressCounts.GetValueOrDefault(tag!, 0)
            )).ToList();

        return studyTopicsResponse;
    }

    public async Task<List<SetQuestionDto>> GetQuestionSet(string categoryTag, string categoryType, int setNumber, string locale)
    {
        if (setNumber < 1) setNumber = 1;

        var totalQuestionsInCategoryTypeWithCategoryTag = await _context.Questions
            .AsNoTracking()
            .CountAsync(q => q.Categories.Any(c => c.Name == categoryType) && q.CategoryTag == categoryTag);

        if (totalQuestionsInCategoryTypeWithCategoryTag == 0) return [];

        var totalSets = (int)Math.Max(1, Math.Round((double)totalQuestionsInCategoryTypeWithCategoryTag / 20));

        if (setNumber > totalSets) setNumber = totalSets;
        
        var baseSize = totalQuestionsInCategoryTypeWithCategoryTag / totalSets;
        var extraQuestions = totalQuestionsInCategoryTypeWithCategoryTag % totalSets;

        int skipAmount;
        int takeAmount;

        if (setNumber <= extraQuestions)
        {
            takeAmount = baseSize + 1;
            skipAmount = (setNumber - 1) * (baseSize + 1);
        }
        else
        {
            takeAmount = baseSize;
            skipAmount = (extraQuestions * (baseSize + 1)) + ((setNumber - extraQuestions - 1) * baseSize);
        }
        
        return await _context.Questions
            .AsNoTracking()
            .Where(q => q.CategoryTag == categoryTag && q.Categories.Any(c => c.Name == categoryType))
            .OrderBy(q => q.QuestionNumber)
            .Skip(skipAmount)
            .Take(takeAmount)
            .Select(q => new SetQuestionDto(
                q.Id,
                q.QuestionNumber,
                locale == "EN" ? q.ContentEn ?? q.ContentPl : 
                locale == "DE" ? q.ContentDe ?? q.ContentPl : 
                locale == "UA" ? q.ContentUa ?? q.ContentPl : q.ContentPl,
            
                locale == "EN" ? q.StaticResponseEn ?? "" : 
                locale == "DE" ? q.StaticResponseDe ?? "" : 
                locale == "UA" ? q.StaticResponseUa ?? "" : q.StaticResponsePl ?? "",
            
                q.Points,
                q.MediaUrl,
                q.CorrectAnswerId,
                q.Answers.Select(a => new SetAnswerDto(
                    a.Id,
                    locale == "EN" ? a.ContentEn ?? a.ContentPl : 
                    locale == "DE" ? a.ContentDe ?? a.ContentPl : 
                    locale == "UA" ? a.ContentUa ?? a.ContentPl : a.ContentPl,
                    q.Id)).ToList()
            ))
            .ToListAsync();
    }
}