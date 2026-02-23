using Domain;

namespace Application.Contracts.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetUsersRefreshToken(string refreshToken);
    Task RemoveUserRefreshTokens(Guid userId);
}