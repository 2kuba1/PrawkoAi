using Application.Common;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Learning.GetQuestionsForSet;

public record GetQuestionsForSet(string CategoryTag, string CategoryType, int SetNumber, string Locale) : IRequest<List<SetQuestionDto>>, ICachableRequest
{
    public string CacheKey => $"study_questions_category_tag_{CategoryTag}_category_name_{CategoryType}_set_{SetNumber}_locale_{Locale}";
    public TimeSpan Expiration => TimeSpan.FromDays(1);
}