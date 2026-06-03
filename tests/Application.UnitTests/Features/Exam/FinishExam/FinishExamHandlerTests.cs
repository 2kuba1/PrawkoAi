using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Features.Exam.FinishExam;
using Application.Models.DTOs;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Transactions;
using FinishExamCommand = Application.Features.Exam.FinishExam.FinishExam;

namespace Application.UnitTests.Features.Exam.FinishExam;

public class FinishExamHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsUnauthorized_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var handlerContext = CreateHandlerContext(Guid.NewGuid());

        // Act
        var act = async () => await handlerContext.Handler.Handle(
            new FinishExamCommand(userId, examSessionId, "PL", CreateAnswers()),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();

        await handlerContext.ExamSessionRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>());

        await handlerContext.UnitOfWork.DidNotReceive()
            .BeginTransactionAsync();
    }

    [Fact]
    public async Task Handle_WhenExamSessionDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var handlerContext = CreateHandlerContext(userId);

        handlerContext.ExamSessionRepository.GetByIdAsync(examSessionId)
            .Returns((ExamSession?)null);

        // Act
        var act = async () => await handlerContext.Handler.Handle(
            new FinishExamCommand(userId, examSessionId, "PL", CreateAnswers()),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();

        await handlerContext.UnitOfWork.DidNotReceive()
            .BeginTransactionAsync();
    }

    [Fact]
    public async Task Handle_WhenExamSessionBelongsToAnotherUser_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var handlerContext = CreateHandlerContext(userId);
        var examSession = new ExamSession
        {
            Id = examSessionId,
            UserId = Guid.NewGuid(),
            StaredAt = DateTime.UtcNow
        };

        handlerContext.ExamSessionRepository.GetByIdAsync(examSessionId)
            .Returns(examSession);

        // Act
        var act = async () => await handlerContext.Handler.Handle(
            new FinishExamCommand(userId, examSessionId, "PL", CreateAnswers()),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();

        await handlerContext.UnitOfWork.DidNotReceive()
            .BeginTransactionAsync();
    }

    [Fact]
    public async Task Handle_WhenExamSessionIsAlreadyFinished_ShouldThrowFinishedExamException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var handlerContext = CreateHandlerContext(userId);
        var examSession = new ExamSession
        {
            Id = examSessionId,
            UserId = userId,
            StaredAt = DateTime.UtcNow.AddMinutes(-5),
            FinishedAt = DateTime.UtcNow
        };

        handlerContext.ExamSessionRepository.GetByIdAsync(examSessionId)
            .Returns(examSession);

        // Act
        var act = async () => await handlerContext.Handler.Handle(
            new FinishExamCommand(userId, examSessionId, "PL", CreateAnswers()),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FinishedExamException>();

        await handlerContext.UnitOfWork.DidNotReceive()
            .BeginTransactionAsync();
    }

    [Fact]
    public async Task Handle_WhenExamSessionExpired_ShouldThrowFinishedExamException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var handlerContext = CreateHandlerContext(userId);
        var examSession = new ExamSession
        {
            Id = examSessionId,
            UserId = userId,
            StaredAt = DateTime.UtcNow.AddMinutes(-27)
        };

        handlerContext.ExamSessionRepository.GetByIdAsync(examSessionId)
            .Returns(examSession);

        // Act
        var act = async () => await handlerContext.Handler.Handle(
            new FinishExamCommand(userId, examSessionId, "PL", CreateAnswers()),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FinishedExamException>();
        examSession.FinishedAt.Should().NotBeNull();

        await handlerContext.UnitOfWork.DidNotReceive()
            .BeginTransactionAsync();
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldUpdateAnswersCommitTransactionAndClearCache()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var answers = CreateAnswers();
        var cancellationToken = CancellationToken.None;
        var handlerContext = CreateHandlerContext(userId);
        var examSession = new ExamSession
        {
            Id = examSessionId,
            UserId = userId,
            StaredAt = DateTime.UtcNow.AddMinutes(-10)
        };
        var results = CreateResults(score: 70, correctAnswersCount: 30);

        handlerContext.ExamSessionRepository.GetByIdAsync(examSessionId)
            .Returns(examSession);

        handlerContext.ExamSessionQuestionRepository.GetExamResultsAsync(examSessionId, "PL")
            .Returns(results);

        handlerContext.ExamSessionRepository
            .CheckIfPassedAndSaveSession(examSession, Arg.Any<DateTime>(), results.Score, results.CorrectAnswersCount)
            .Returns(true);

        handlerContext.Cache.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var request = new FinishExamCommand(userId, examSessionId, "PL", answers, "B");

        // Act
        var result = await handlerContext.Handler.Handle(request, cancellationToken);

        // Assert
        result.Should().BeSameAs(results);
        result.IsPassed.Should().BeTrue();
        result.StartedAt.Should().Be(examSession.StaredAt);
        result.FinishedAt.Should().NotBeNull();

        await handlerContext.UnitOfWork.Received(1)
            .BeginTransactionAsync();

        await handlerContext.ExamSessionQuestionRepository.Received(1)
            .BulkUpdateAnswersAsync(examSessionId, answers);

        await handlerContext.UserAnswerRepository.Received(answers.Count)
            .CreateAsync(Arg.Any<UserAnswer>());

        await handlerContext.UserRepository.Received(1)
            .UpdateStreak(userId, handlerContext.Cache, "B");

        await handlerContext.UnitOfWork.Received(1)
            .SaveChangesAsync(cancellationToken);

        await handlerContext.UnitOfWork.Received(1)
            .CommitTransactionAsync();

        await handlerContext.UnitOfWork.DidNotReceive()
            .RollbackTransactionAsync();

        await handlerContext.Cache.Received(1)
            .RemoveAsync($"user_stats_{userId}", cancellationToken);
    }

    [Fact]
    public async Task Handle_WhenOperationInsideTransactionFails_ShouldRollbackAndThrowTransactionException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var examSessionId = Guid.NewGuid();
        var answers = CreateAnswers();
        var handlerContext = CreateHandlerContext(userId);
        var examSession = new ExamSession
        {
            Id = examSessionId,
            UserId = userId,
            StaredAt = DateTime.UtcNow.AddMinutes(-10)
        };

        handlerContext.ExamSessionRepository.GetByIdAsync(examSessionId)
            .Returns(examSession);

        handlerContext.ExamSessionQuestionRepository
            .BulkUpdateAnswersAsync(examSessionId, answers)
            .Returns<Task>(_ => throw new InvalidOperationException("Bulk update failed"));

        // Act
        var act = async () => await handlerContext.Handler.Handle(
            new FinishExamCommand(userId, examSessionId, "PL", answers),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TransactionException>();

        await handlerContext.UnitOfWork.Received(1)
            .BeginTransactionAsync();

        await handlerContext.UnitOfWork.Received(1)
            .RollbackTransactionAsync();

        await handlerContext.UnitOfWork.DidNotReceive()
            .CommitTransactionAsync();

        await handlerContext.Cache.DidNotReceive()
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static HandlerContext CreateHandlerContext(Guid currentUserId)
    {
        var httpContextAccessor = HttpContextHelper.WithUserId(currentUserId);
        var examSessionRepository = Substitute.For<IExamSessionRepository>();
        var examSessionQuestionRepository = Substitute.For<IExamSessionQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        var userRepository = Substitute.For<IUserRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var cache = Substitute.For<IDistributedCache>();

        var handler = new FinishExamHandler(
            httpContextAccessor,
            examSessionRepository,
            examSessionQuestionRepository,
            userAnswerRepository,
            userRepository,
            unitOfWork,
            cache);

        return new HandlerContext(
            handler,
            examSessionRepository,
            examSessionQuestionRepository,
            userAnswerRepository,
            userRepository,
            unitOfWork,
            cache);
    }

    private static List<UserAnswerSubmissionDto> CreateAnswers()
    {
        return
        [
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
            new(Guid.NewGuid(), null, DateTime.UtcNow)
        ];
    }

    private static ExamResultsDto CreateResults(int score, int correctAnswersCount)
    {
        return new ExamResultsDto
        {
            CorrectAnswersCount = correctAnswersCount,
            CorrectAnswers = [],
            IncorrectAnswers = [],
            Unanswered = [],
            Score = score
        };
    }

    private sealed record HandlerContext(
        FinishExamHandler Handler,
        IExamSessionRepository ExamSessionRepository,
        IExamSessionQuestionRepository ExamSessionQuestionRepository,
        IUserAnswerRepository UserAnswerRepository,
        IUserRepository UserRepository,
        IUnitOfWork UnitOfWork,
        IDistributedCache Cache);
}
