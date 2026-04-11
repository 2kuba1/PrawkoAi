using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IUserAiProgressRepository : IGenericRepository<UserAiProgress>
{
    Task<string?> GetAiProgress(Guid userId);
}