using Application.Contracts.Repositories;
using Application.Features.Users.DashboardData;
using Application.UnitTests.TestHelpers;
using Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Users.DashboardData;

public class DashboardDataHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAuthorized_ShouldReturnDashboardDataFromRepositories()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = "B";

        var categoryRepository = Substitute.For<ICategoryRepository>();
        var questionRepository = Substitute.For<IQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var examSessionRepository = Substitute.For<IExamSessionRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var httpContextAccessor = HttpContextHelper.WithUserId(userId);

        categoryRepository.GetUserWorsePerformingCategory(userId).Returns("Signs");
        userAnswerRepository.GetUniqueQuestionsAnsweredCount(userId, category).Returns(42);
        questionRepository.GetQuestionsCountOfCategory(category).Returns(120);
        userAnswerRepository.TodayQuestionsAnsweredCount(userId).Returns(8);
        examSessionRepository.GetAverageExamScore(userId).Returns(64.5f);
        userRepository.GetStreak(userId).Returns(5);

        var handler = new DashboardDataHandler(
            categoryRepository,
            questionRepository,
            userAnswerRepository,
            examSessionRepository,
            userRepository,
            httpContextAccessor);

        // Act
        var result = await handler.Handle(new Application.Features.Users.DashboardData.DashboardData(userId, category), CancellationToken.None);

        // Assert
        result.WorstPerformingCategory.Should().Be("Signs");
        result.MaxQuestionsCount.Should().Be(120);
        result.QuestionsAnsweredCount.Should().Be(42);
        result.Streak.Should().Be(5);
        result.AverageScore.Should().Be(64.5f);
        result.TodayQuestionsAnsweredCount.Should().Be(8);
    }

    [Fact]
    public async Task Handle_WhenUserIsUnauthorized_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryRepository = Substitute.For<ICategoryRepository>();
        var questionRepository = Substitute.For<IQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var examSessionRepository = Substitute.For<IExamSessionRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var httpContextAccessor = HttpContextHelper.WithUserId(Guid.NewGuid());

        var handler = new DashboardDataHandler(
            categoryRepository,
            questionRepository,
            userAnswerRepository,
            examSessionRepository,
            userRepository,
            httpContextAccessor);

        // Act
        var act = async () => await handler.Handle(
            new Application.Features.Users.DashboardData.DashboardData(userId),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();

        await categoryRepository.DidNotReceive()
            .GetUserWorsePerformingCategory(Arg.Any<Guid>());

        await userAnswerRepository.DidNotReceive()
            .GetUniqueQuestionsAnsweredCount(Arg.Any<Guid>(), Arg.Any<string>());

        await questionRepository.DidNotReceive()
            .GetQuestionsCountOfCategory(Arg.Any<string>());
    }
}
