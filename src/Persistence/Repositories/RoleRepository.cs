using Application.Contracts.Repositories;
using Domain;
using Persistence.Database;

namespace Persistence.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }
}