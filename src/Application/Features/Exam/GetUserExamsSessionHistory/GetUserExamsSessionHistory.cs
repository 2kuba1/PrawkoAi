using Application.Models;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Exam.GetUserExamsSessionHistory;

public record GetUserExamsSessionHistory(Guid UserId, int PageNumber, int PageSize) : IRequest<PagedList<ExamSessionHistory>>;