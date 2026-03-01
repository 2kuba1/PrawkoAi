using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Seeders;

internal static class RolesSeeder
{
    internal static async Task Seed(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        
        if(await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>()
        {
            new Role()
            {
                Name = "User",
                Id = Guid.NewGuid()
            },
            new Role()
            {
                Name = "Admin",
                Id = Guid.NewGuid()
            },
            new Role()
            {
                Name = "DrivingSchoolOwner",
                Id = Guid.NewGuid()
            },
        };
        
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}