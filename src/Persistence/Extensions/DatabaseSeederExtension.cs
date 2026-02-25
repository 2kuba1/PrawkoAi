using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Database;
using Persistence.Seeders;

namespace Persistence.Extensions;

public static class DatabaseSeederExtension
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context =  scope.ServiceProvider.GetService<AppDbContext>();
        if (context != null)
        {
            await context.Database.EnsureCreatedAsync();
            await QuestionsSeeder.Seed(context);
            await CategoriesSeeder.Seed(context);
            await RolesSeeder.Seed(context);
        }
    }
}