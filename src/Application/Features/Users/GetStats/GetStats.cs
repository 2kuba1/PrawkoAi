using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Users.GetStats;

public record GetStats(Guid UserId) : IRequest<UserStatsDto>;