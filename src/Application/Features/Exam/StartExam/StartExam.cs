using Application.Models;
using Domain;
using MediatR;

namespace Application.Features.Exam.StartExam;

public record StartExam(Guid UserId) : IRequest<StartExamResponse>;