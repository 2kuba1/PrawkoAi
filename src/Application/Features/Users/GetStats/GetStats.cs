using Application.Common;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Users.GetStats;

public record GetStats(Guid UserId) : IRequest<UserStatsDto>, ICachableRequest
{
    public string CacheKey => $"user_stats_{UserId}";
    public TimeSpan Expiration => TimeSpan.FromHours(6);
}