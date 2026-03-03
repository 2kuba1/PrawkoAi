using Application.Models;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IExamSessionQuestionRepository : IGenericRepository<ExamSessionQuestion>
{
    Task SaveExamSessionQuestionsAsync(ExamQuestions examSessionQuestions, Guid examSessionId);
    Task UpdateExamSessionQuestionAsync(ExamSessionQuestion examSessionQuestion, Guid examSessionId, Guid selectedAnswerId);
    Task<ExamSessionQuestion?> GetByQuestionAndSessionIdAsync(Guid questionId, Guid examSessionId);
}