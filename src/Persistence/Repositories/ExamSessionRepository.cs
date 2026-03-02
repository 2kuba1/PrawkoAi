using Application.Contracts.Repositories;
using Domain.Entities;
using Persistence.Database;

namespace Persistence.Repositories;

public class ExamSessionRepository : GenericRepository<ExamSession>, IExamSessionRepository
{
    public ExamSessionRepository(AppDbContext context) : base(context)
    {
    }
}