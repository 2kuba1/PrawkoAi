using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Exam.FinishExam;

public record FinishExam(Guid UserId, Guid ExamSessionId, string Locale, List<UserAnswerSubmissionDto> Answers) : IRequest<ExamResultsDto>;