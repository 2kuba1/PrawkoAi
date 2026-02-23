using Domain;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
                     .Where(q => q.State is EntityState.Added or EntityState.Modified))
        {
            entry.Entity.UpdatedAt = GetValidUtcNow();

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = GetValidUtcNow();
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
    
    private static DateTime GetValidUtcNow()
    {
        var utcNow = DateTime.UtcNow;

        if (utcNow == DateTime.MaxValue || utcNow == DateTime.MinValue)
        {
            utcNow = DateTime.UtcNow.AddMinutes(-1);
        }

        return utcNow;
    }
}