using Application.Contracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class UserAiProgressRepository : GenericRepository<UserAiProgress>, IUserAiProgressRepository
{
    private readonly AppDbContext _context;

    public UserAiProgressRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<string?> GetAiProgress(Guid userId)
        => await _context.UserAiProgresses
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => p.Content)
            .FirstOrDefaultAsync();
        
 
}