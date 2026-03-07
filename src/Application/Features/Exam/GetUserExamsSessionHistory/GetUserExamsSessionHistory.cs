using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Exam.GetUserExamsSessionHistory;

public record GetUserExamsSessionHistory(Guid UserId) : IRequest<List<ExamSessionHistory>>;