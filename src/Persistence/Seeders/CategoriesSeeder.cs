using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Seeders;

internal static class CategoriesSeeder
{
    internal static async Task Seed(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>()
        {
            new Category()
            {
                Name = "A",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "B",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "B1",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "C",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "C1",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "D",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "T",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "AM",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "A1",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "A2",
                Id = Guid.NewGuid(),
            },
            new Category()
            {
                Name = "D1",
                Id = Guid.NewGuid(),
            }
        };
        
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
}