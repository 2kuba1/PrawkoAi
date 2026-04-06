using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Questions.GetMediaAndStaticResponse;

public record GetQuestionAdditionalData(Guid QuestionId, string Locale) : IRequest<Models.DTOs.GetQuestionAdditionalDataDto>;
