using Application.Contracts.Repositories;
using Domain;
using Persistence.Database;

namespace Persistence.Repositories;

public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
{
    public AnswerRepository(AppDbContext context) : base(context)
    {
    }
}