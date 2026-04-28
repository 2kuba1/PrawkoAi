using Application.Contracts.Repositories;
using Application.Models;
using Application.Models.DTOs;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Questions.SearchForQuestion;

internal sealed class SearchForQuestionHandler : IRequestHandler<SearchForQuestion, PagedList<FoundQuestionsDto>>
{
    private readonly IQuestionRepository _questionRepository;

    public SearchForQuestionHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    
    public async Task<PagedList<FoundQuestionsDto>> Handle(SearchForQuestion request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question) || request.Question.Length < 3)
            throw new NotFoundException("Question must be longer than 3 characters");

        var query = request.Question.ToLower();

        var results = await _questionRepository.SearchForQuestions(query, request.Locale, request.CategoryType, request.PageSize, request.PageNumber);
        return results;
    }
}