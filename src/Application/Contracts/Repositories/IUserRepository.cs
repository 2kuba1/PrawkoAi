using Domain;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> FindUserByEmailAsync(string email);
    Task<User?> FindUserByDeviceIdAsync(string deviceId);
    Task<User> CreateNewGuestAsync(string deviceId);
}