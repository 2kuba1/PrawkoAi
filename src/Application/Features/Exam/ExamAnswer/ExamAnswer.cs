using MediatR;

namespace Application.Features.Exam.ExamAnswer;

public record ExamAnswer(Guid ExamSessionId, Guid QuestionId, Guid SelectedAnswerId, Guid UserId) : IRequest<Unit>;