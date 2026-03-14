using Application.Contracts.Repositories;
using Application.Shared;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Answers.AnswerToQuestion;

internal sealed class AnswerToQuestionHandler : IRequestHandler<AnswerToQuestion, Unit>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAnswerRepository _answerRepository;
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IQuestionRepository _questionRepository;

    public AnswerToQuestionHandler(IHttpContextAccessor  httpContextAccessor, IAnswerRepository answerRepository, IUserAnswerRepository userAnswerRepository, IQuestionRepository questionRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _answerRepository = answerRepository;
        _userAnswerRepository = userAnswerRepository;
        _questionRepository = questionRepository;
    }
    
    public async Task<Unit> Handle(AnswerToQuestion request, CancellationToken cancellationToken)
    {
        if(request.UserId != Utils.GetCurrentUserId(_httpContextAccessor))
            throw new UnauthorizedException("You are not authorized to answer as that user");
        
        if(!await _questionRepository.CheckIfQuestionExists(request.QuestionId))
            throw new NotFoundException("Question does not exist");

        if (!await _answerRepository.IsAnswerValid(request.SelectedAnswerId, request.QuestionId)) 
            throw new NotFoundException("Answer does not exist");
        
        await _userAnswerRepository.CreateAsync(new UserAnswer()
        {
            UserId = request.UserId,
            SelectedAnswerId = request.SelectedAnswerId,
            QuestionId = request.QuestionId
        });
        
        return Unit.Value;
    }
}