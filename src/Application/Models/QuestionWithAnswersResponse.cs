using Domain;
using Domain.Entities;
using MediatR;

namespace Application.Models;

public record QuestionWithAnswersResponse(Question Question, IEnumerable<Answer> Answers);