using Application.Models;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IExamSessionQuestionRepository : IGenericRepository<ExamSessionQuestion>
{
    Task SaveExamSessionQuestions(ExamQuestions examSessionQuestions, Guid examSessionId);
}