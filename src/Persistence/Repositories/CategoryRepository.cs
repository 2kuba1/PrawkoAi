using Application.Contracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Category?> GetCategoryByName(string categoryName)
        => await _context.Categories.FirstOrDefaultAsync(x => x.Name == categoryName);

    public async Task<List<string>> GetAllCategoriesNames()
        =>await _context.Categories.Select(x => x.Name).ToListAsync();

    public async Task<string?> GetUserWorsePerformingCategory(Guid userId)
    {
        var worstCategory = await _context.UserAnswers
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .GroupBy(u => u.Question.CategoryTag)
            .Select(g => new
            {
                CategoryName = g.Key,
                Accuracy = (double)g.Count(x => x.SelectedAnswerId == x.Question.CorrectAnswerId) / g.Count() * 100
            })
            .OrderBy(x => x.Accuracy)
            .ThenByDescending(x => _context.UserAnswers.Count(ua => ua.UserId == userId && ua.Question.CategoryTag == x.CategoryName))
            .Select(x => x.CategoryName)
            .FirstOrDefaultAsync();

        return worstCategory;
    }
}