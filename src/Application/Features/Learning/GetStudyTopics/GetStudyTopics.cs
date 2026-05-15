using Application.Common;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Learning.GetStudyTopics;

public record GetStudyTopics(Guid UserId, string Category) : IRequest<List<GetStudyTopicsResponeDto>>, ICachableRequest
{
    public string CacheKey => "study_topics";
    public TimeSpan Expiration => TimeSpan.FromDays(1);
}