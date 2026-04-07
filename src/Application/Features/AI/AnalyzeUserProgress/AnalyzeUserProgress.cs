using MediatR;

namespace Application.Features.AI.AnalyzeUserProgress;

public record AnalyzeUserProgress(Guid UserId) : IRequest<string>;