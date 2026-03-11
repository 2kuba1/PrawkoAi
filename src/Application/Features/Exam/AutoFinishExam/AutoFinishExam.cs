using MediatR;

namespace Application.Features.Exam.AutoFinishExam;

public record AutoFinishExam(Guid ExamSessionId) : IRequest<Unit>;