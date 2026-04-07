using Application.Models.DTOs;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IUserAnswerRepository : IGenericRepository<UserAnswer>
{
    Task<List<UserLastAnswersDto>> GetUserLastAnswers(Guid userId, int answerCount);
}