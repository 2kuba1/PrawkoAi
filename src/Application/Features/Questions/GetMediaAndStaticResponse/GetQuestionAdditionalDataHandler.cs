using Application.Contracts.Repositories;
using Application.Models.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Questions.GetMediaAndStaticResponse;

internal sealed class GetQuestionAdditionalDataHandler : IRequestHandler<GetQuestionAdditionalData, Models.DTOs.GetQuestionAdditionalDataDto>
{
    private readonly IQuestionRepository _questionRepository;

    public GetQuestionAdditionalDataHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    
    public async Task<Models.DTOs.GetQuestionAdditionalDataDto> Handle(GetQuestionAdditionalData request, CancellationToken cancellationToken)
    {
        var result = await _questionRepository.GetQuestionAdditionalData(request.QuestionId, request.Locale);

        if (result is null)
            throw new QuestionNotFoundException($"Question {request.QuestionId} not found"); 
                
        return result;
    }
}