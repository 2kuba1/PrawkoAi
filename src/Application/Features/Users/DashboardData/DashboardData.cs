using Application.Common;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Users.DashboardData;

public record DashboardData(Guid UserId, string Category = "B") : IRequest<DashboardDataDto>, ICachableRequest
{
    public string CacheKey => $"dashboard_data_{UserId}_category_{Category}";
    public TimeSpan Expiration => TimeSpan.FromHours(1);
}