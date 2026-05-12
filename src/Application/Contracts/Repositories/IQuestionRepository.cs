using Application.Models;
using Application.Models.DTOs;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<Question?> GetRandomQuestionByCategory(Guid categoryId);
    Task<bool> CheckIfQuestionExists(Guid questionId);
    Task<ExamQuestions> GetExamSimulationQuestions(string category, string? locale);
    Task<GetQuestionAdditionalDataDto?> GetQuestionAdditionalData(Guid questionId, string locale);
    Task<AiRequiredDataDto?> GetRequiredAiData(Guid questionId, string locale);
    Task<List<GetStudyTopicsResponeDto>> GetUserLearningProgressAndTopicsCount(Guid userId, string category);
    Task<List<SetQuestionDto>> GetQuestionSet(string categoryTag, string categoryType, int setNumber, string locale);
    Task<PagedList<FoundQuestionsDto>> SearchForQuestions(string query, string locale, string categoryType, int  pageSize, int pageNumber);
    Task<QuestionDto?> GetQuestionWithAnswers(float questionNumber,  string locale);
    Task<int> GetQuestionsCountOfCategory(string category = "B");
}