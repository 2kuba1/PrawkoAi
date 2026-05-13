using Application.Models;
using Application.Models.DTOs;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IExamSessionRepository : IGenericRepository<ExamSession>
{
    bool CheckIfPassedAndSaveSession(ExamSession examSession, DateTime finishedAt, int score, int correctAnswerCount);
    Task<PagedList<ExamSessionHistory>> GetUserExamsSessionHistory(Guid userId, int pageNumber, int pageSize);
    Task<List<int?>> GetLastExamsScores(Guid userId);
    Task<float> GetAverageExamScore(Guid userId);
}