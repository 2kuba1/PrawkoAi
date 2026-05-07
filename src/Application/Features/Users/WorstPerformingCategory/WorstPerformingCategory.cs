using MediatR;

namespace Application.Features.Users.WorstPerformingCategory;

public record WorstPerformingCategory(Guid UserId) : IRequest<string>;