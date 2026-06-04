using Domain.Entities;
using FluentAssertions;
using Persistence.Repositories;

namespace Persistence.IntegrationTests.Repositories;

[Collection(IntegrationTestCollection.Name)]
public class QuestionRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public QuestionRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetQuestionWithAnswers_WhenQuestionExists_ShouldReturnLocalizedQuestionWithAnswers()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var category = new Category { Id = Guid.NewGuid(), Name = "B" };
        var question = CreateQuestion(
            questionNumber: 101,
            contentPl: "Polska treść pytania",
            contentEn: "English question content",
            categoryTag: "Signs",
            category);

        await AddQuestionWithAnswersAsync(context, question,
            ("Tak", "Yes"),
            ("Nie", "No"));

        var repository = new QuestionRepository(context);

        // Act
        var result = await repository.GetQuestionWithAnswers(101, "EN");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(question.Id);
        result.Content.Should().Be("English question content");
        result.StaticResponse.Should().Be("English explanation");
        result.CorrectAnswerId.Should().Be(question.CorrectAnswerId);
        result.Answers.Should().HaveCount(2);
        result.Answers.Select(x => x.Content).Should().Contain(["Yes", "No"]);
    }

    [Fact]
    public async Task SearchForQuestions_WhenQueryMatchesEnglishContent_ShouldReturnPagedResults()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var category = new Category { Id = Guid.NewGuid(), Name = "B" };

        await AddQuestionWithAnswersAsync(context,
            CreateQuestion(201, "Pieszy przy przejściu", "Pedestrian near crossing", "Crossings", category),
            ("Tak", "Yes"),
            ("Nie", "No"));

        await AddQuestionWithAnswersAsync(context,
            CreateQuestion(202, "Sygnalizacja świetlna", "Traffic lights question", "Lights", category),
            ("Tak", "Yes"),
            ("Nie", "No"));

        var repository = new QuestionRepository(context);

        // Act
        var result = await repository.SearchForQuestions(
            query: "pedestrian",
            locale: "en",
            categoryType: "B",
            pageSize: 10,
            pageNumber: 1);

        // Assert
        result.Items.Should().ContainSingle();
        result.Items[0].QuestionNumber.Should().Be(201);
        result.Items[0].Content.Should().Be("Pedestrian near crossing");
        result.TotalCount.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetQuestionSet_ShouldReturnQuestionsFromRequestedCategoryTagAndCategory()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();
        await using var context = _fixture.CreateDbContext();

        var categoryB = new Category { Id = Guid.NewGuid(), Name = "B" };
        var categoryA = new Category { Id = Guid.NewGuid(), Name = "A" };

        await AddQuestionWithAnswersAsync(context,
            CreateQuestion(301, "Pierwsze pytanie", "First question", "Signs", categoryB),
            ("Tak", "Yes"),
            ("Nie", "No"));

        await AddQuestionWithAnswersAsync(context,
            CreateQuestion(302, "Drugie pytanie", "Second question", "Signs", categoryB),
            ("Tak", "Yes"),
            ("Nie", "No"));

        await AddQuestionWithAnswersAsync(context,
            CreateQuestion(303, "Inna kategoria", "Other category", "Signs", categoryA),
            ("Tak", "Yes"),
            ("Nie", "No"));

        await AddQuestionWithAnswersAsync(context,
            CreateQuestion(304, "Inny tag", "Other tag", "Lights", categoryB),
            ("Tak", "Yes"),
            ("Nie", "No"));

        var repository = new QuestionRepository(context);

        // Act
        var result = await repository.GetQuestionSet("Signs", "B", setNumber: 1, locale: "EN");

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.QuestionNumber).Should().Equal(301, 302);
        result.Select(x => x.QuestionContent).Should().Equal("First question", "Second question");
        result.All(x => x.Answers.Count == 2).Should().BeTrue();
    }

    private static Question CreateQuestion(
        float questionNumber,
        string contentPl,
        string contentEn,
        string categoryTag,
        Category category)
    {
        return new Question
        {
            Id = Guid.NewGuid(),
            ContentPl = contentPl,
            ContentEn = contentEn,
            StaticResponsePl = "Polskie wyjaśnienie",
            StaticResponseEn = "English explanation",
            CategoryTag = categoryTag,
            CategoryType = category.Name,
            QuestionNumber = questionNumber,
            Points = 3,
            MediaUrl = "media.mp4",
            Categories = [category]
        };
    }

    private static async Task AddQuestionWithAnswersAsync(
        Persistence.Database.AppDbContext context,
        Question question,
        params (string ContentPl, string ContentEn)[] answers)
    {
        question.Answers = answers
            .Select(answer => new Answer
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id,
                ContentPl = answer.ContentPl,
                ContentEn = answer.ContentEn
            })
            .ToList();

        context.Questions.Add(question);
        await context.SaveChangesAsync();

        question.CorrectAnswerId = question.Answers[0].Id;
        await context.SaveChangesAsync();
    }
}
