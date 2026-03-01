using Application.Contracts.Repositories;
using Domain;
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
}