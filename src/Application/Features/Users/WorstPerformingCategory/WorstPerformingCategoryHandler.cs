using Application.Contracts.Repositories;
using Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.WorstPerformingCategory;

internal sealed class WorstPerformingCategoryHandler : IRequestHandler<Users.WorstPerformingCategory.WorstPerformingCategory, string>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorstPerformingCategoryHandler(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<string> Handle(Users.WorstPerformingCategory.WorstPerformingCategory request, CancellationToken cancellationToken)
    {
        if(Utils.GetCurrentUserId(_httpContextAccessor) != request.UserId)
            throw new UnauthorizedAccessException("You can not access this users data");

        var categoryName = _categoryRepository.GetUserWorsePerformingCategory(request.UserId);
        return categoryName;
    }
}