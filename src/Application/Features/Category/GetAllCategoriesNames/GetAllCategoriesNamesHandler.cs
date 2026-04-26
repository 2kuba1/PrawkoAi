using Application.Contracts.Repositories;
using MediatR;

namespace Application.Features.Category.GetAllCategoriesNames;

internal sealed class GetAllCategoriesNamesHandler : IRequestHandler<GetAllCategoriesNames, List<string>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesNamesHandler(ICategoryRepository  categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<List<string>> Handle(GetAllCategoriesNames request, CancellationToken cancellationToken)
    {
        var names = await _categoryRepository.GetAllCategoriesNames();
        return names;
    }
}