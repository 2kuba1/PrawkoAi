using Domain;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryByName(string categoryName);
    Task<List<string>> GetAllCategoriesNames();
}