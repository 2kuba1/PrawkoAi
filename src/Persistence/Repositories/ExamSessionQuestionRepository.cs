using Application.Contracts.Repositories;
using Domain.Entities;
using Persistence.Database;

namespace Persistence.Repositories;

public class ExamSessionQuestionRepository : GenericRepository<ExamSessionQuestion>, IExamSessionQuestionRepository
{
    public ExamSessionQuestionRepository(AppDbContext context) : base(context)
    {
    }
}