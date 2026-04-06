using MediatR;

namespace Application.Features.AI.GetAiExplanationStream;

public record GetAiExplanationStream(Guid QuestionId, string UserQuery, string Locale) : IStreamRequest<string>;