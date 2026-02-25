using Application.Models;
using MediatR;

namespace Application.Features.Questions.GetRandomQuestionByCategory;

public record GetRandomQuestionByCategory(string CategoryName) : IRequest<QuestionWithAnswersResponse>;