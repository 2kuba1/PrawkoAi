using Application.Contracts.Repositories;
using Application.Shared;
using Domain;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Answers.AnswerToQuestion;

public class AnswerToQuestionHandler : IRequestHandler<AnswerToQuestion, Unit>
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
        if(Guid.Parse(request.UserId) != Utils.GetCurrentUserId(_httpContextAccessor))
            throw new UnauthorizedException("You are not authorized to answer as that user");
        
        if(!await _questionRepository.CheckIfQuestionExists(Guid.Parse(request.QuestionId)))
            throw new NotFoundException("Question does not exist");

        if (!await _answerRepository.IsAnswerValid(Guid.Parse(request.SelectedAnswerId), Guid.Parse(request.QuestionId))) 
            throw new NotFoundException("Answer does not exist");
        
        await _userAnswerRepository.CreateAsync(new UserAnswer()
        {
            UserId = Guid.Parse(request.UserId),
            SelectedAnswerId = Guid.Parse(request.SelectedAnswerId),
            QuestionId = Guid.Parse(request.QuestionId)
        });
        
        return Unit.Value;
    }
}