using Application.Common;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Questions.GetMediaAndStaticResponse;

public record GetQuestionAdditionalData(Guid QuestionId, string Locale) : IRequest<GetQuestionAdditionalDataDto>, ICachableRequest
{
    public string CacheKey => $"question_additional_data_{QuestionId}_locale_{Locale}";
    public TimeSpan Expiration => TimeSpan.FromDays(1);
}
