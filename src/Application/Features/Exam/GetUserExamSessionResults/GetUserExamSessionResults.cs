using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Exam.GetUserExamSessionResults;

public record GetUserExamSessionResults(Guid UserId, Guid ExamSessionId, string Locale) : IRequest<ExamResultsDto>;