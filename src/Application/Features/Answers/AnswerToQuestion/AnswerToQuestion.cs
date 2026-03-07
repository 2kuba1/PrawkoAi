using MediatR;

namespace Application.Features.Answers.AnswerToQuestion;

public record AnswerToQuestion(Guid QuestionId, Guid UserId, Guid SelectedAnswerId) : IRequest<Unit>;