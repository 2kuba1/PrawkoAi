using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Seeders;

internal static class QuestionsSeeder
{
    internal static async Task Seed(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Questions.AnyAsync())
            return;
    }
}