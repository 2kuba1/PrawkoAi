using Application.Contracts.Repositories;
using Application.Features.Answers.LogUserAnswersInSet;
using Application.Models.DTOs;
using Application.UnitTests.TestHelpers;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace Application.UnitTests.Features.Answers;

public class LogUserAnswersInSetHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAuthorized_ShouldCreateSetAnswersAndUpdateStreak()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var answers = CreateAnswers();
        var categoryName = "B";
        var cancellationToken = CancellationToken.None;

        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var httpContext = HttpContextHelper.WithUserId(userId);
        var distributedCache = Substitute.For<IDistributedCache>();

        userRepository
            .UpdateStreak(userId, distributedCache, categoryName)
            .Returns(Task.CompletedTask);

        userAnswerRepository
            .CreateSetAnswers(userId, answers)
            .Returns(Task.CompletedTask);

        distributedCache
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var handler = new LogUserAnswersInSetHandler(
            userAnswerRepository,
            userRepository,
            httpContext,
            distributedCache);

        // Act
        await handler.Handle(
            new LogUserAnswersInSet(userId, answers, categoryName),
            cancellationToken);

        // Assert
        await userRepository.Received(1)
            .UpdateStreak(userId, distributedCache, categoryName);

        await userAnswerRepository.Received(1)
            .CreateSetAnswers(userId, answers);

        await distributedCache.Received(1)
            .RemoveAsync($"user_stats_{userId}", cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenUserIsUnauthorized_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var answers = CreateAnswers();
        var categoryName = "B";

        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var httpContext = HttpContextHelper.WithUserId(Guid.NewGuid());
        var distributedCache = Substitute.For<IDistributedCache>();

        var handler = new LogUserAnswersInSetHandler(
            userAnswerRepository,
            userRepository,
            httpContext,
            distributedCache);

        // Act
        var act = async () => await handler.Handle(
            new LogUserAnswersInSet(userId, answers, categoryName),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();

        await userRepository.DidNotReceive()
            .UpdateStreak(Arg.Any<Guid>(), Arg.Any<IDistributedCache>(), Arg.Any<string>());

        await userAnswerRepository.DidNotReceive()
            .CreateSetAnswers(Arg.Any<Guid>(), Arg.Any<List<UserSetAnswerDto>>());

        await distributedCache.DidNotReceive()
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUpdatingStreakFails_ShouldThrowAndNotCreateAnswers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var answers = CreateAnswers();
        var categoryName = "B";

        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var httpContext = HttpContextHelper.WithUserId(userId);
        var distributedCache = Substitute.For<IDistributedCache>();

        userRepository
            .UpdateStreak(userId, distributedCache, categoryName)
            .Returns<Task>(_ => throw new InvalidOperationException("Streak update failed"));

        var handler = new LogUserAnswersInSetHandler(
            userAnswerRepository,
            userRepository,
            httpContext,
            distributedCache);

        // Act
        var act = async () => await handler.Handle(
            new LogUserAnswersInSet(userId, answers, categoryName),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        await userRepository.Received(1)
            .UpdateStreak(userId, distributedCache, categoryName);

        await userAnswerRepository.DidNotReceive()
            .CreateSetAnswers(Arg.Any<Guid>(), Arg.Any<List<UserSetAnswerDto>>());

        await distributedCache.DidNotReceive()
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCreatingSetAnswersFails_ShouldThrowAndNotClearCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var answers = CreateAnswers();
        var categoryName = "B";

        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var httpContext = HttpContextHelper.WithUserId(userId);
        var distributedCache = Substitute.For<IDistributedCache>();

        userRepository
            .UpdateStreak(userId, distributedCache, categoryName)
            .Returns(Task.CompletedTask);

        userAnswerRepository
            .CreateSetAnswers(userId, answers)
            .Returns<Task>(_ => throw new InvalidOperationException("Set answers creation failed"));

        var handler = new LogUserAnswersInSetHandler(
            userAnswerRepository,
            userRepository,
            httpContext,
            distributedCache);

        // Act
        var act = async () => await handler.Handle(
            new LogUserAnswersInSet(userId, answers, categoryName),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();

        await userRepository.Received(1)
            .UpdateStreak(userId, distributedCache, categoryName);

        await userAnswerRepository.Received(1)
            .CreateSetAnswers(userId, answers);

        await distributedCache.DidNotReceive()
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static List<UserSetAnswerDto> CreateAnswers()
    {
        return
        [
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow)
        ];
    }
}
