using Application.Contracts.Repositories;
using Domain;
using Persistence.Database;

namespace Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}