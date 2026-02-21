using Application.Contracts.Repositories;
using Domain;
using Persistence.Database;

namespace Persistence.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext context) : base(context)
    {
    }
}