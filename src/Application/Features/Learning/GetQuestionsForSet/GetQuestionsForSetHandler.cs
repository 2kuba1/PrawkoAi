using Application.Contracts.Repositories;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Learning.GetQuestionsForSet;

internal sealed class GetQuestionsForSetHandler : IRequestHandler<GetQuestionsForSet, List<SetQuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;

    public GetQuestionsForSetHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    
    public async Task<List<SetQuestionDto>> Handle(GetQuestionsForSet request, CancellationToken cancellationToken)
    {
        var result = await _questionRepository.GetQuestionSet(request.CategoryTag, request.CategoryType, request.SetNumber, request.Locale);
        return result;
    }
}