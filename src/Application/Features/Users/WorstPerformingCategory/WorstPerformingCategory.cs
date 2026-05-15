using Application.Common;
using MediatR;

namespace Application.Features.Users.WorstPerformingCategory;

public record WorstPerformingCategory(Guid UserId) : IRequest<string>, ICachableRequest
{
    public string CacheKey => $"worstPerformingCategory_{UserId}";
    public TimeSpan Expiration => TimeSpan.FromHours(1);
}