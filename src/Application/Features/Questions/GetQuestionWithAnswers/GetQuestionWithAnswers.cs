using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Questions.GetQuestionWithAnswers;

public record GetQuestionWithAnswers(float QuestionNumber, string Locale) : IRequest<QuestionDto>;