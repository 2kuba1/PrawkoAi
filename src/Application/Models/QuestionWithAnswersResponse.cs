using Domain;
using MediatR;

namespace Application.Models;

public record QuestionWithAnswersResponse(Question Question, IEnumerable<Answer> Answers);