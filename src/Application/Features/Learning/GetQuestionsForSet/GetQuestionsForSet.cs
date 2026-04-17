using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Learning.GetQuestionsForSet;

public record GetQuestionsForSet(string CategoryTag, string CategoryType, int SetNumber, string Locale) : IRequest<List<SetQuestionDto>>;