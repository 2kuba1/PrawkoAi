using Domain;

namespace Application.Contracts.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> FindUserByEmailAsync(string email);
}