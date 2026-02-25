using Domain;

namespace Application.Contracts.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetRoleByName(string roleName);
}