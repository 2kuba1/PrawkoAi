using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Questions.GetQuestionWithAnswers;

internal sealed class GetQuestionWithAnswersHandler : IRequestHandler<GetQuestionWithAnswers, QuestionDto>
{
    private readonly IQuestionRepository _questionRepository;

    public GetQuestionWithAnswersHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    
    public async Task<QuestionDto> Handle(GetQuestionWithAnswers request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetQuestionWithAnswers(request.QuestionNumber, request.Locale);
        return question ?? throw new NotFoundException($"Question with number: {request.QuestionNumber} not found");
    }
}