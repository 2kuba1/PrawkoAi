using Application.Contracts.Repositories;
using Application.Features.Answers.AnswerToQuestion;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Answers;

public class AnswerToQuestionHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserIsAuthorizedAndAnswerIsValid_ShouldCreateUserAnswer()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var answerId = Guid.NewGuid();
        
        var httpContextAccessor = HttpContextHelper.WithUserId(userId);

        var answerRepository = Substitute.For<IAnswerRepository>();
        var questionRepository = Substitute.For<IQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        
        questionRepository.CheckIfQuestionExists(questionId).Returns(true);
        answerRepository.IsAnswerValid(answerId, questionId).Returns(true);

        var handler = new AnswerToQuestionHandler(
            httpContextAccessor,
            answerRepository,
            userAnswerRepository,
            questionRepository);
        
        //Act
        await handler.Handle(new AnswerToQuestion(questionId, userId, answerId), CancellationToken.None);
        
        //Assert
        await userAnswerRepository.Received(1)
            .CreateAsync(Arg.Is<UserAnswer>(x => 
                x.UserId == userId &&
                x.QuestionId == questionId &&
                x.SelectedAnswerId == answerId));   
    }

    [Fact]
    public async Task Handle_WhenUserIsUnAuthorizedToAnswerThisQuestion_ShouldThrowUnauthorizedException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var answerId = Guid.NewGuid();
        
        var httpContextAccessor = HttpContextHelper.WithUserId(Guid.NewGuid());
        
        var answerRepository = Substitute.For<IAnswerRepository>();
        var questionRepository = Substitute.For<IQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();

        var handler = new AnswerToQuestionHandler(
            httpContextAccessor,
            answerRepository,
            userAnswerRepository,
            questionRepository);
        
        //Act
        var act = async () => await handler.Handle(new AnswerToQuestion(questionId, userId, answerId), CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        
        await questionRepository.DidNotReceive()
            .CheckIfQuestionExists(Arg.Any<Guid>());
        
        await answerRepository.DidNotReceive()
            .IsAnswerValid(Arg.Any<Guid>(), Arg.Any<Guid>());
        
        await userAnswerRepository.DidNotReceive()
            .CreateAsync(Arg.Any<UserAnswer>());
    }
    
    [Fact]
    public async Task Handle_WhenQuestionDoesNotExist_ShouldThrowNotFoundException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var answerId = Guid.NewGuid();
        
        var httpContextAccessor = HttpContextHelper.WithUserId(userId);
        
        var answerRepository = Substitute.For<IAnswerRepository>();
        var questionRepository = Substitute.For<IQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();
        
        questionRepository.CheckIfQuestionExists(questionId).Returns(false);

        var handler = new AnswerToQuestionHandler(
            httpContextAccessor,
            answerRepository,
            userAnswerRepository,
            questionRepository);
        
        //Act
        var act = async () => await handler.Handle(new AnswerToQuestion(questionId, userId, answerId), CancellationToken.None);
        
        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
        
        await questionRepository.Received(1)
            .CheckIfQuestionExists(Arg.Is(questionId));
        
        await answerRepository.DidNotReceive()
            .IsAnswerValid(Arg.Any<Guid>(), Arg.Any<Guid>());
        
        await userAnswerRepository.DidNotReceive()
            .CreateAsync(Arg.Any<UserAnswer>());
    }

    [Fact]
    public async Task Handle_WhenAnswerIsInvalid_ShouldThrowNotFoundException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var selectedAnswerId = Guid.NewGuid();
        
        var httpContextAccessor = HttpContextHelper.WithUserId(userId);
        
        var answerRepository = Substitute.For<IAnswerRepository>();
        var questionRepository = Substitute.For<IQuestionRepository>();
        var userAnswerRepository = Substitute.For<IUserAnswerRepository>();

        questionRepository.CheckIfQuestionExists(questionId).Returns(true);
        answerRepository.IsAnswerValid(selectedAnswerId, questionId).Returns(false);
        
        var handler = new AnswerToQuestionHandler(
            httpContextAccessor,
            answerRepository,
            userAnswerRepository,
            questionRepository);
        
        //Act
        var act = async () => await handler.Handle(new AnswerToQuestion(questionId, userId, selectedAnswerId), CancellationToken.None);
        
        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
        
        await questionRepository.Received(1)
            .CheckIfQuestionExists(Arg.Is(questionId));
        
        await answerRepository.Received(1)
            .IsAnswerValid(selectedAnswerId, questionId);
        
        await userAnswerRepository.DidNotReceive()
            .CreateAsync(Arg.Any<UserAnswer>());
    }
}