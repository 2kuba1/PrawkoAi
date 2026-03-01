using Application.Contracts.Repositories;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Database;

namespace Persistence.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetUsersRefreshToken(string refreshToken)
    {
        var token = await _context.RefreshTokens.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);
        return token;
    }

    public async Task RemoveUserRefreshTokens(Guid userId)
    {
        await _context.RefreshTokens
            .Where(r => r.UserId == userId)
            .ExecuteDeleteAsync();
    }
}