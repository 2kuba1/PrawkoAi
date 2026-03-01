using Domain;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IAnswerRepository : IGenericRepository<Answer>
{
    Task<bool> IsAnswerValid(Guid selectedAnswerId, Guid questionId);
}