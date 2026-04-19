using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Answers.LogUserAnswersInSet;

public record LogUserAnswersInSet(Guid UserId, List<UserSetAnswerDto> Answers) : IRequest<Unit>;