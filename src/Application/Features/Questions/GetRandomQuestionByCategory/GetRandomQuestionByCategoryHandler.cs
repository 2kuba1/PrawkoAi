using Application.Contracts.Repositories;
using Application.Models;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Questions.GetRandomQuestionByCategory;

internal sealed class GetRandomQuestionByCategoryHandler : IRequestHandler<GetRandomQuestionByCategory, QuestionWithAnswersResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IQuestionRepository _questionRepository;

    public GetRandomQuestionByCategoryHandler(ICategoryRepository categoryRepository, IQuestionRepository questionRepository)
    {
        _categoryRepository = categoryRepository;
        _questionRepository = questionRepository;
    }
    
    public async Task<QuestionWithAnswersResponse> Handle(GetRandomQuestionByCategory request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryByName(request.CategoryName);

        if (category is null)
            throw new NotFoundException("Category doesn't exist");

        var question = await _questionRepository.GetRandomQuestionByCategory(category.Id);

        return question is null ? throw new NotFoundException("No matching questions") : new QuestionWithAnswersResponse(question, question.Answers);
    }
}