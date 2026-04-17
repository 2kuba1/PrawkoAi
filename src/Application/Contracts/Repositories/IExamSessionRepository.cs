using Application.Models.DTOs;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IExamSessionRepository : IGenericRepository<ExamSession>
{
    bool CheckIfPassedAndSaveSession(ExamSession examSession, DateTime finishedAt, int score, int correctAnswerCount);
    Task<List<ExamSessionHistory>> GetUserExamsSessionHistory(Guid userId);
    Task<List<int?>> GetLastExamsScores(Guid userId);
}