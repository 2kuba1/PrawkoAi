using Application.Common;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Questions.GetQuestionWithAnswers;

public record GetQuestionWithAnswers(float QuestionNumber, string Locale) : IRequest<QuestionDto>, ICachableRequest
{
    public string CacheKey => $"question_with_answers_{QuestionNumber}_locale_{Locale}";
    public TimeSpan Expiration =>  TimeSpan.FromDays(1);
}