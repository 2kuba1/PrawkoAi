using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Users.DashboardData;

public record DashboardData(Guid UserId, string Category = "B") : IRequest<DashboardDataDto>;