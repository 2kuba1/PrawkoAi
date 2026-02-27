using MediatR;

namespace Application.Features.Answers.AnswerToQuestion;

public record AnswerToQuestion(string QuestionId, string UserId, string SelectedAnswerId) : IRequest<Unit>;