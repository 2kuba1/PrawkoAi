using System.Globalization;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Seeders;

internal static class QuestionsSeeder
{
    internal static async Task Seed(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Questions.AnyAsync())
            return;

        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "questions.txt");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var lines = await File.ReadAllLinesAsync(filePath);

        context.ChangeTracker.AutoDetectChangesEnabled = false;

        var existingCategories = await context.Categories.ToListAsync();
        var categoryDictionary = existingCategories.ToDictionary(c => c.Name, c => c);

        var questionsToProcess = new List<(Domain.Entities.Question Question, string CorrectRaw)>();

        foreach (var line in lines)
        {
            var parts = line.Split('@');
            if (parts.Length < 10) continue;

            var questionNumber = ParseFloatSafe(parts[0]);
            var points = ParseFloatSafe(parts[8]);

            var questionText = parts[1].Trim();
            var answerA = IsInvalid(parts[2]) ? null : parts[2].Trim();
            var answerB = IsInvalid(parts[3]) ? null : parts[3].Trim();
            var answerC = IsInvalid(parts[4]) ? null : parts[4].Trim();
            var correctRaw = parts[5].Trim().ToUpper();
            var media = IsInvalid(parts[6]) ? null : parts[6].Trim();
            var structureScope = IsInvalid(parts[7]) ? null : parts[7].Trim();
            var categoriesRaw = parts[9].Trim();

            var question = new Question
            {
                Id = Guid.NewGuid(),
                QuestionNumber = questionNumber,
                Content = questionText,
                StructureScope = structureScope,
                Points = points,
                MediaUrl = media,
                CorrectAnswerId = null, 
                Answers = new List<Answer>(),
                Categories = new List<Category>()
            };

            if (!string.IsNullOrWhiteSpace(answerA) || !string.IsNullOrWhiteSpace(answerB) || !string.IsNullOrWhiteSpace(answerC))
            {
                if (!string.IsNullOrWhiteSpace(answerA)) question.Answers.Add(new Answer { Id = Guid.NewGuid(), Content = answerA, QuestionId = question.Id });
                if (!string.IsNullOrWhiteSpace(answerB)) question.Answers.Add(new Answer { Id = Guid.NewGuid(), Content = answerB, QuestionId = question.Id });
                if (!string.IsNullOrWhiteSpace(answerC)) question.Answers.Add(new Answer { Id = Guid.NewGuid(), Content = answerC, QuestionId = question.Id });
            }
            else
            {
                question.Answers.Add(new Answer { Id = Guid.NewGuid(), Content = "Tak", QuestionId = question.Id });
                question.Answers.Add(new Answer { Id = Guid.NewGuid(), Content = "Nie", QuestionId = question.Id });
            }

            if (!string.IsNullOrWhiteSpace(categoriesRaw))
            {
                var names = categoriesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim());
                foreach (var name in names)
                {
                    if (!categoryDictionary.TryGetValue(name, out var category))
                    {
                        category = new Category { Id = Guid.NewGuid(), Name = name };
                        categoryDictionary[name] = category;
                        context.Categories.Add(category);
                    }
                    question.Categories.Add(category);
                }
            }

            questionsToProcess.Add((question, correctRaw));
        }

        var allQuestions = questionsToProcess.Select(x => x.Question).ToList();
        await context.Questions.AddRangeAsync(allQuestions);
        await context.SaveChangesAsync();
        
        foreach (var item in questionsToProcess)
        {
            var q = item.Question;
            var raw = item.CorrectRaw;

            if (q.Answers.Count == 2)
            {
                q.CorrectAnswerId = (raw == "T" || raw == "TAK")
                    ? q.Answers.First(a => a.Content == "Tak").Id
                    : q.Answers.First(a => a.Content == "Nie").Id;
            }
            else
            {
                var index = raw switch { "A" => 0, "B" => 1, "C" => 2, _ => -1 };
                if (index >= 0 && index < q.Answers.Count)
                    q.CorrectAnswerId = q.Answers[index].Id;
            }
        }

        context.ChangeTracker.AutoDetectChangesEnabled = true;
        await context.SaveChangesAsync();
    }

    private static float ParseFloatSafe(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "nan")
            return 0f;

        if (float.TryParse(value.Replace(',', '.').Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return float.IsNaN(result) || float.IsInfinity(result) ? 0f : result;
        }
        return 0f;
    }

    private static bool IsInvalid(string value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "nan";
}