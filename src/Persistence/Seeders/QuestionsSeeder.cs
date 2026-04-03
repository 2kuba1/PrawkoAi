using System.Globalization;
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

        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "questions_processed.txt");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        context.ChangeTracker.AutoDetectChangesEnabled = false;

        var categoryDictionary = await context.Categories.ToDictionaryAsync(c => c.Name, c => c);
        var questionsToProcess = new List<(Question Question, string CorrectRaw)>();

        var lines = await File.ReadAllLinesAsync(filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var d = line.Split('@').ToList();
            
            if (d.Count < 28) continue;

            var questionId = Guid.NewGuid();
            var question = new Question
            {
                Id = questionId,
                // 0: ID
                QuestionNumber = ParseFloatSafe(d[0]),
                // 1: Question PL
                ContentPl = d[1].Trim(),
                
                // 11: Question EN (indeks 10)
                ContentEn = GetValueSafe(d, 10),
                // 15: Question DE (indeks 14)
                ContentDe = GetValueSafe(d, 14),
                // 19: Question UA
                ContentUa = GetValueSafe(d, 18),
                
                // 7: Media
                MediaUrl = GetValueSafe(d, 6),
                // 8: Category
                CategoryType = GetValueSafe(d, 7),
                // 9: Points
                Points = ParseFloatSafe(GetValueSafe(d, 8)),
                
                // 23: Ai Context 
                AiContext = GetValueSafe(d, 22),       
                // 24: Kategoria/Tag
                CategoryTag = GetValueSafe(d, 23),     
                // 25: Static response pl
                StaticResponsePl = GetValueSafe(d, 24), 
                // 26: Static response en
                StaticResponseEn = GetValueSafe(d, 25), 
                // 27: Static response de
                StaticResponseDe = GetValueSafe(d, 26), 
                // 28: Static response ua
                StaticResponseUa = GetValueSafe(d, 27), 
                
                Answers = new List<Answer>(),
                Categories = new List<Category>()
            };

            // PL: 2,3,4 | EN: 11,12,13 | DE: 15,16,17 | UA: 19,20,21
            var isMultipleChoice = !IsInvalid(d[2]) || !IsInvalid(d[3]) || !IsInvalid(d[4]);

            if (isMultipleChoice)
            {
                question.Answers.Add(CreateAnswer(question.Id, d[2], d[11], d[15], d[19]));
                question.Answers.Add(CreateAnswer(question.Id, d[3], d[12], d[16], d[20]));
                question.Answers.Add(CreateAnswer(question.Id, d[4], d[13], d[17], d[21]));
            }
            else
            {
                question.Answers.Add(new Answer { Id = Guid.NewGuid(), QuestionId = question.Id, ContentPl = "Tak", ContentEn = "Yes", ContentDe = "Ja", ContentUa = "Так" });
                question.Answers.Add(new Answer { Id = Guid.NewGuid(), QuestionId = question.Id, ContentPl = "Nie", ContentEn = "No", ContentDe = "Nein", ContentUa = "НІ" });
            }

            var categoriesRaw = GetValueSafe(d, 9);
            if (!string.IsNullOrWhiteSpace(categoriesRaw))
            {
                var names = categoriesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(c => c.Trim().ToUpper());

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

            // 6: Correct answer
            questionsToProcess.Add((question, GetValueSafe(d, 5).ToUpper()));
        }

        await context.Questions.AddRangeAsync(questionsToProcess.Select(x => x.Question));
        await context.SaveChangesAsync();

        foreach (var item in questionsToProcess)
        {
            var q = item.Question;
            var raw = item.CorrectRaw;

            if (q.Answers.Count == 2) // Y/N
            {
                var isTrue = raw == "T" || raw == "TAK";
                q.CorrectAnswerId = isTrue 
                    ? q.Answers.First(a => a.ContentPl == "Tak").Id 
                    : q.Answers.First(a => a.ContentPl == "Nie").Id;
            }
            else // A/B/C
            {
                var index = raw switch { "A" => 0, "B" => 1, "C" => 2, _ => -1 };
                if (index >= 0 && index < q.Answers.Count)
                    q.CorrectAnswerId = q.Answers[index].Id;
            }
        }

        context.ChangeTracker.AutoDetectChangesEnabled = true;
        await context.SaveChangesAsync();
    }

    private static Answer CreateAnswer(Guid qId, string pl, string en, string de, string uk) => new()
    {
        Id = Guid.NewGuid(),
        QuestionId = qId,
        ContentPl = IsInvalid(pl) ? "" : pl.Trim(),
        ContentEn = IsInvalid(en) ? null : en.Trim(),
        ContentDe = IsInvalid(de) ? null : de.Trim(),
        ContentUa = IsInvalid(uk) ? null : uk.Trim()
    };

    private static string GetValueSafe(List<string> data, int index)
    {
        if (index < 0 || index >= data.Count) return "";
        var val = data[index];
        return IsInvalid(val) ? "" : val.Trim();
    }

    private static float ParseFloatSafe(string value)
    {
        if (IsInvalid(value)) return 0f;
        var cleaned = value.Replace(',', '.').Trim();
        return float.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0f;
    }

    private static bool IsInvalid(string value) =>
        string.IsNullOrWhiteSpace(value) || 
        value.Trim().ToLower() == "nan" || 
        value.Trim().ToLower() == "null";
}