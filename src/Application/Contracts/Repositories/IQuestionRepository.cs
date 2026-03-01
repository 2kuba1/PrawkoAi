using Application.Models;
using Domain;

namespace Application.Contracts.Repositories;

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<Question?> GetRandomQuestionByCategory(Guid categoryId);
    Task<bool> CheckIfQuestionExists(Guid questionId);
    Task<ExamQuestions> GetExamSimulationQuestions();
}