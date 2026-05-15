using Domain;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Contracts.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> FindUserByEmailAsync(string email);
    Task<User?> FindUserByDeviceIdAsync(string deviceId);
    Task<User> CreateNewGuestAsync(string deviceId);
    Task<int> GetStreak(Guid userId);
    Task UpdateStreak(Guid userId, IDistributedCache cache, string categoryName = "B");
}