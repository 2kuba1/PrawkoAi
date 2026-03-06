using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IExamSessionRepository : IGenericRepository<ExamSession>
{
    Task<bool> CheckIfPassedAndSaveSession(ExamSession examSession, DateTime finishedAt, int score);
}