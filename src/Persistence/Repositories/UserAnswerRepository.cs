using Application.Contracts.Repositories;
using Domain;
using Persistence.Database;

namespace Persistence.Repositories;

public class UserAnswerRepository : GenericRepository<UserAnswer>, IUserAnswerRepository
{
    public UserAnswerRepository(AppDbContext context) : base(context)
    {
    }
}